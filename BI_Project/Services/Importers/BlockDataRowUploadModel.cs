using System.Collections.Generic;

namespace bicen.Services.Importers
{
    public class BlockDataRowUploadModel
    {
        public BlockDataRowUploadModel() : base()
        {

        }

        public int Month { set; get; }

        public int Year { set; get; }

        public int File { set; get; }

        public string DataString { set; get; }

        //public List<DNT_QMKLTN_HA1820> ListDNT { set; get; }
    }
}