using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



using System.Collections.Generic;

namespace siredd_vf.Services{
    public class CrsProperties
    {
        public string name { get; set; }
    }

    public class Crs
    {
        public string type { get; set; }
        public CrsProperties properties { get; set; }
    }

    public class DataEtab
    {
        public int id { get; set; }
        public string cd_reg { get; set; }
        public string ll_reg { get; set; }
        public string cd_prov { get; set; }
        public string ll_prov { get; set; }
        public string cd_com { get; set; }
        public string ll_com { get; set; }
        public string cd_etab { get; set; }
        public string nom_etabl { get; set; }
        public string ll_netab { get; set; }
        //public double gprs_longi { get; set; }
        //public double gprs_latit { get; set; }
        //public double @long { get; set; }
        //public double lat { get; set; }
        //public int correction { get; set; }
        //public int Non_corrig { get; set; }
        //public double x { get; set; }
        //public double y { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<double>> coordinates { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public DataEtab properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class RootObject
    {
        public string type { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
        public List<Feature> features { get; set; }
    }
}
