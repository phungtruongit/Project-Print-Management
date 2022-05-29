using System;
using System.Runtime.Serialization;

namespace PrintManagement.AdminApp.Models {
    [DataContract]
    public class DataPoint {
        public DataPoint() { }

        public DataPoint(string label, double y) {
            this.Label = label;
            this.Y = y;
        }

        public string OrderNumber { get; set; }
        public string TimeItem { get; set; }
        public string ContentItem { get; set; }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}
