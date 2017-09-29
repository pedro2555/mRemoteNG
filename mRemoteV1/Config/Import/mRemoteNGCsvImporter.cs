using mRemoteNG.Config.DataProviders;
using mRemoteNG.Config.Serializers;
using mRemoteNG.Container;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace mRemoteNG.Config.Import
{
    public class mRemoteNGCsvImporter : mRemoteNGImporter
    {
        public new void Import(string fileName, ContainerInfo destinationContainer)
        {
            var dataProvider = new FileDataProvider(fileName);
            var xmlString = dataProvider.Load();
            var csvConnectionsDeserializer = new CsvConnectionsDeserializerMremotengFormat(xmlString);
            var connectionTreeModel = csvConnectionsDeserializer.Deserialize();

            var rootImportContainer = new ContainerInfo { Name = Path.GetFileNameWithoutExtension(fileName) };
            rootImportContainer.AddChildRange(connectionTreeModel.RootNodes.First().Children.ToArray());
            destinationContainer.AddChild(rootImportContainer);
        }
    }
}
