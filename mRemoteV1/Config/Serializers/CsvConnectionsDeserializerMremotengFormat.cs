using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mRemoteNG.Tree;
using System.IO;
using CsvHelper;
using mRemoteNG.Connection;
using mRemoteNG.Container;

namespace mRemoteNG.Config.Serializers
{
    class CsvConnectionsDeserializerMremotengFormat : IDeserializer
    {
        private TextReader textReader;

        public CsvConnectionsDeserializerMremotengFormat(string csvString)
        {
            textReader = new StringReader(csvString);
        }

        public ConnectionTreeModel Deserialize()
        {
            ConnectionTreeModel m = new ConnectionTreeModel();
            
            var csvReader = new CsvReader(textReader);
            csvReader.Configuration.RegisterClassMap<ConnectionInfoCsvMap>();
            csvReader.Configuration.HasHeaderRecord = true;

            csvReader.Read();

            var records = csvReader.GetRecord<ConnectionInfo>();


            ///
            /// TODO: we should rebuild the three structure at this point
            ///
            ContainerInfo recordContainers = records as ContainerInfo;
            ConnectionTreeModel result = new ConnectionTreeModel();
            result.AddRootNode(recordContainers);

            return result;
        }
    }
}
