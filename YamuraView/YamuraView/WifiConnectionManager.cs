using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace YamuraView
{
    /// <summary>
    /// Uses the native Windows WLAN API (wlanapi.dll) to check which wireless
    /// network is currently connected, and to connect to a different network.
    /// If a password is supplied, a WPA2-PSK profile is installed (or overwritten)
    /// under the SSID's name before connecting; otherwise an existing
    /// Windows-saved profile with a matching name is used.
    /// </summary>
    internal static class WifiConnectionManager
    {
        private const int ERROR_SUCCESS = 0;
        private const uint WLAN_INTERFACE_STATE_CONNECTED = 1;

        [DllImport("wlanapi.dll")]
        private static extern int WlanOpenHandle(uint dwClientVersion, IntPtr pReserved, out uint pdwNegotiatedVersion, out IntPtr phClientHandle);

        [DllImport("wlanapi.dll")]
        private static extern int WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

        [DllImport("wlanapi.dll")]
        private static extern int WlanEnumInterfaces(IntPtr hClientHandle, IntPtr pReserved, out IntPtr ppInterfaceList);

        [DllImport("wlanapi.dll")]
        private static extern int WlanQueryInterface(IntPtr hClientHandle, ref Guid pInterfaceGuid, WLAN_INTF_OPCODE OpCode, IntPtr pReserved, out uint pdwDataSize, out IntPtr ppData, IntPtr pWlanOpcodeValueType);

        [DllImport("wlanapi.dll")]
        private static extern int WlanConnect(IntPtr hClientHandle, ref Guid pInterfaceGuid, ref WLAN_CONNECTION_PARAMETERS pConnectionParameters, IntPtr pReserved);

        [DllImport("wlanapi.dll", CharSet = CharSet.Unicode)]
        private static extern int WlanSetProfile(IntPtr hClientHandle, ref Guid pInterfaceGuid, uint dwFlags,
            string strProfileXml, string? strAllUserProfileSecurity, [MarshalAs(UnmanagedType.Bool)] bool bOverwrite,
            IntPtr pReserved, out uint pdwReasonCode);

        [DllImport("wlanapi.dll")]
        private static extern void WlanFreeMemory(IntPtr pMemory);

        private enum WLAN_INTF_OPCODE
        {
            wlan_intf_opcode_current_connection = 7
        }

        private enum WLAN_CONNECTION_MODE
        {
            wlan_connection_mode_profile = 0
        }

        private enum DOT11_BSS_TYPE
        {
            dot11_BSS_type_infrastructure = 1
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WLAN_INTERFACE_INFO
        {
            public Guid InterfaceGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strInterfaceDescription;
            public uint isState;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_INTERFACE_INFO_LIST_HEADER
        {
            public uint dwNumberOfItems;
            public uint dwIndex;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DOT11_SSID
        {
            public uint uSSIDLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] ucSSID;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_ASSOCIATION_ATTRIBUTES
        {
            public DOT11_SSID dot11Ssid;
            public uint dot11BssType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] dot11Bssid;
            public uint dot11PhyType;
            public uint uDot11PhyIndex;
            public uint wlanSignalQuality;
            public uint ulRxRate;
            public uint ulTxRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_SECURITY_ATTRIBUTES
        {
            public bool bSecurityEnabled;
            public bool bOneXEnabled;
            public uint dot11AuthAlgorithm;
            public uint dot11CipherAlgorithm;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WLAN_CONNECTION_ATTRIBUTES
        {
            public uint isState;
            public uint wlanConnectionMode;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strProfileName;
            public WLAN_ASSOCIATION_ATTRIBUTES wlanAssociationAttributes;
            public WLAN_SECURITY_ATTRIBUTES wlanSecurityAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_CONNECTION_PARAMETERS
        {
            public WLAN_CONNECTION_MODE wlanConnectionMode;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string strProfile;
            public IntPtr pDot11Ssid;
            public IntPtr pDesiredBssidList;
            public DOT11_BSS_TYPE dot11BssType;
            public uint dwFlags;
        }

        /// <summary>
        /// Returns the SSID the first connected wireless adapter is associated
        /// with, or null if no adapter is currently connected.
        /// </summary>
        public static string? GetConnectedSsid()
        {
            if (WlanOpenHandle(2, IntPtr.Zero, out _, out IntPtr clientHandle) != ERROR_SUCCESS)
            {
                return null;
            }
            try
            {
                if (WlanEnumInterfaces(clientHandle, IntPtr.Zero, out IntPtr interfaceListPtr) != ERROR_SUCCESS)
                {
                    return null;
                }
                try
                {
                    foreach (Guid interfaceGuid in EnumerateInterfaceGuids(interfaceListPtr))
                    {
                        Guid guid = interfaceGuid;
                        if (WlanQueryInterface(clientHandle, ref guid, WLAN_INTF_OPCODE.wlan_intf_opcode_current_connection,
                                IntPtr.Zero, out _, out IntPtr dataPtr, IntPtr.Zero) != ERROR_SUCCESS)
                        {
                            continue;
                        }
                        try
                        {
                            WLAN_CONNECTION_ATTRIBUTES attrs = Marshal.PtrToStructure<WLAN_CONNECTION_ATTRIBUTES>(dataPtr);
                            if (attrs.isState == WLAN_INTERFACE_STATE_CONNECTED)
                            {
                                return Encoding.UTF8.GetString(attrs.wlanAssociationAttributes.dot11Ssid.ucSSID, 0,
                                    (int)attrs.wlanAssociationAttributes.dot11Ssid.uSSIDLength);
                            }
                        }
                        finally
                        {
                            WlanFreeMemory(dataPtr);
                        }
                    }
                }
                finally
                {
                    WlanFreeMemory(interfaceListPtr);
                }
            }
            finally
            {
                WlanCloseHandle(clientHandle, IntPtr.Zero);
            }
            return null;
        }

        /// <summary>
        /// Checks whether the machine is already connected to <paramref name="ssid"/>.
        /// If not, and <paramref name="password"/> is non-empty, installs a WPA2-PSK
        /// profile named after the SSID with that password (overwriting any existing
        /// profile of the same name) and connects to it. If <paramref name="password"/>
        /// is empty, connects using an existing Windows-saved profile with the same
        /// name instead. Waits up to <paramref name="timeoutMs"/> for the connection
        /// to complete. Returns true if already connected or the connection succeeded
        /// within the timeout.
        /// </summary>
        public static bool EnsureConnected(string ssid, string password = "", int timeoutMs = 8000)
        {
            if (string.IsNullOrWhiteSpace(ssid))
            {
                return true;
            }
            if (string.Equals(GetConnectedSsid(), ssid, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (WlanOpenHandle(2, IntPtr.Zero, out _, out IntPtr clientHandle) != ERROR_SUCCESS)
            {
                return false;
            }
            try
            {
                if (WlanEnumInterfaces(clientHandle, IntPtr.Zero, out IntPtr interfaceListPtr) != ERROR_SUCCESS)
                {
                    return false;
                }
                Guid[] guids;
                try
                {
                    guids = EnumerateInterfaceGuids(interfaceListPtr).ToArray();
                }
                finally
                {
                    WlanFreeMemory(interfaceListPtr);
                }

                string? profileXml = string.IsNullOrEmpty(password) ? null : BuildWpa2PskProfileXml(ssid, password);

                foreach (Guid interfaceGuid in guids)
                {
                    Guid guid = interfaceGuid;
                    if (profileXml != null)
                    {
                        WlanSetProfile(clientHandle, ref guid, 0, profileXml, null, true, IntPtr.Zero, out _);
                    }
                    WLAN_CONNECTION_PARAMETERS connectionParams = new WLAN_CONNECTION_PARAMETERS
                    {
                        wlanConnectionMode = WLAN_CONNECTION_MODE.wlan_connection_mode_profile,
                        strProfile = ssid,
                        pDot11Ssid = IntPtr.Zero,
                        pDesiredBssidList = IntPtr.Zero,
                        dot11BssType = DOT11_BSS_TYPE.dot11_BSS_type_infrastructure,
                        dwFlags = 0
                    };
                    if (WlanConnect(clientHandle, ref guid, ref connectionParams, IntPtr.Zero) == ERROR_SUCCESS)
                    {
                        break;
                    }
                }
            }
            finally
            {
                WlanCloseHandle(clientHandle, IntPtr.Zero);
            }

            int waited = 0;
            const int pollIntervalMs = 500;
            while (waited < timeoutMs)
            {
                Thread.Sleep(pollIntervalMs);
                waited += pollIntervalMs;
                if (string.Equals(GetConnectedSsid(), ssid, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Builds a WLAN profile XML document (WPA2-PSK/AES, infrastructure mode)
        /// for the given SSID and passphrase. Values are placed via XElement so
        /// XML-special characters in the SSID or password are escaped safely.
        /// </summary>
        private static string BuildWpa2PskProfileXml(string ssid, string password)
        {
            XNamespace ns = "http://www.microsoft.com/networking/WLAN/profile/v1";
            XDocument doc = new XDocument(
                new XDeclaration("1.0", null, null),
                new XElement(ns + "WLANProfile",
                    new XElement(ns + "name", ssid),
                    new XElement(ns + "SSIDConfig",
                        new XElement(ns + "SSID",
                            new XElement(ns + "name", ssid))),
                    new XElement(ns + "connectionType", "ESS"),
                    new XElement(ns + "connectionMode", "auto"),
                    new XElement(ns + "MSM",
                        new XElement(ns + "security",
                            new XElement(ns + "authEncryption",
                                new XElement(ns + "authentication", "WPA2PSK"),
                                new XElement(ns + "encryption", "AES"),
                                new XElement(ns + "useOneX", "false")),
                            new XElement(ns + "sharedKey",
                                new XElement(ns + "keyType", "passPhrase"),
                                new XElement(ns + "protected", "false"),
                                new XElement(ns + "keyMaterial", password))))));
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        private static IEnumerable<Guid> EnumerateInterfaceGuids(IntPtr interfaceListPtr)
        {
            WLAN_INTERFACE_INFO_LIST_HEADER header = Marshal.PtrToStructure<WLAN_INTERFACE_INFO_LIST_HEADER>(interfaceListPtr);
            IntPtr itemsStart = IntPtr.Add(interfaceListPtr, Marshal.SizeOf<WLAN_INTERFACE_INFO_LIST_HEADER>());
            int itemSize = Marshal.SizeOf<WLAN_INTERFACE_INFO>();
            for (int i = 0; i < header.dwNumberOfItems; i++)
            {
                IntPtr itemPtr = IntPtr.Add(itemsStart, i * itemSize);
                WLAN_INTERFACE_INFO info = Marshal.PtrToStructure<WLAN_INTERFACE_INFO>(itemPtr);
                yield return info.InterfaceGuid;
            }
        }
    }
}
