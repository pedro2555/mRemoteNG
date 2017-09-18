using System;
using System.Linq;
using mRemoteNG.Connection;
using mRemoteNG.Container;
using mRemoteNG.Security;
using mRemoteNG.Tree;
using mRemoteNG.Tree.Root;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Windows.Forms;
using mRemoteNG.App;

namespace mRemoteNG.Config.Serializers
{
    public sealed class ConnectionInfoCsvMap : CsvClassMap<ConnectionInfo>
    {
        public ConnectionInfoCsvMap()
        {
            Map(m => m.Hostname).Name("Hostname");
            Map(m => m.Protocol).Name("Protocol");
            Map(m => m.PuttySession).Name("PuttySession");
            Map(m => m.Port).Name("Port");
            Map(m => m.UseConsoleSession).Name("UseConsoleSession");
            Map(m => m.UseCredSsp).Name("UseCredSsp");
            Map(m => m.RenderingEngine).Name("RenderingEngine");
            Map(m => m.ICAEncryptionStrength).Name("ICAEncryptionStrength");
            Map(m => m.RDPAuthenticationLevel).Name("RDPAuthenticationLevel");
            Map(m => m.LoadBalanceInfo).Name("LoadBalanceInfo");
            Map(m => m.Colors).Name("Colors");
            Map(m => m.Resolution).Name("Resolution");
            Map(m => m.AutomaticResize).Name("AutomaticResize");
            Map(m => m.DisplayWallpaper).Name("DisplayWallpaper");
            Map(m => m.DisplayThemes).Name("DisplayThemes");
            Map(m => m.EnableFontSmoothing).Name("EnableFontSmoothing");
            Map(m => m.EnableDesktopComposition).Name("EnableDesktopComposition");
            Map(m => m.CacheBitmaps).Name("CacheBitmaps");
            Map(m => m.RedirectDiskDrives).Name("RedirectDiskDrives");
            Map(m => m.RedirectPorts).Name("RedirectPorts");
            Map(m => m.RedirectPrinters).Name("RedirectPrinters");
            Map(m => m.RedirectSmartCards).Name("RedirectSmartCards");
            Map(m => m.RedirectSound).Name("RedirectSound");
            Map(m => m.RedirectKeys).Name("RedirectKeys");
            Map(m => m.PreExtApp).Name("PreExtApp");
            Map(m => m.PostExtApp).Name("PostExtApp");
            Map(m => m.MacAddress).Name("MacAddress");
            Map(m => m.UserField).Name("UserField");
            Map(m => m.ExtApp).Name("ExtApp");
            Map(m => m.VNCCompression).Name("VNCCompression");
            Map(m => m.VNCEncoding).Name("VNCEncoding");
            Map(m => m.VNCAuthMode).Name("VNCAuthMode");
            Map(m => m.VNCProxyType).Name("VNCProxyType");
            Map(m => m.VNCProxyIP).Name("VNCProxyIP");
            Map(m => m.VNCProxyPort).Name("VNCProxyPort");
            Map(m => m.VNCProxyUsername).Name("VNCProxyUsername");
            Map(m => m.VNCProxyPassword).Name("VNCProxyPassword");
            Map(m => m.VNCColors).Name("VNCColors");
            Map(m => m.VNCSmartSizeMode).Name("VNCSmartSizeMode");
            Map(m => m.VNCViewOnly).Name("VNCViewOnly");
        }
    }

    public class CsvConnectionsSerializerMremotengFormat : ISerializer<string>
    {
        private TextWriter writer = null;
        private CsvWriter csvWriter = null;

        public SaveFilter SaveFilter { get; set; }

        public string Serialize(ConnectionInfo serializationTarget)
        {
            // initialize CsvHelper stuff
            if (csvWriter == null)
            {
                // write to a StringWriter to return to the interface
                if (writer == null)
                    writer = new StringWriter();

                csvWriter = new CsvWriter(writer);
                csvWriter.Configuration.RegisterClassMap<ConnectionInfoCsvMap>();
                csvWriter.Configuration.HasHeaderRecord = true;
            }

            ContainerInfo nodeContainer = serializationTarget as ContainerInfo;
            if (nodeContainer == null)
                try
                {
                    csvWriter.WriteRecord(serializationTarget); // base case, node is not a container
                }
                catch (Exception ex)
                {
                    Runtime.MessageCollector.AddExceptionStackTrace($"CsvConnectionsSerializerMremotengFormat.Serialize() failed.", ex);
                }
            else
                foreach (ConnectionInfo childNode in nodeContainer.Children)
                    Serialize(childNode); // recursively serialize the children nodes

            // clean up
            writer.Flush();
            // output is actually ignored when the call originates from a child node
            // but the StringWriter keeps it anyway, so the last call always returns everything
            return writer.ToString();
        }

        public string Serialize(ConnectionTreeModel connectionTreeModel)
        {
            var rootNode = connectionTreeModel.RootNodes.First(node => node is RootNodeInfo);
            return Serialize(rootNode);
        }

    }
}