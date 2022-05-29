using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace PrintManagement.AdminApp.Reports {
    public partial class Rpt_UserStatistic : DevExpress.XtraReports.UI.XtraReport {
        public Rpt_UserStatistic(object datasource) {
            InitializeComponent();
            this.DataSource = datasource;
        }
    }
}
