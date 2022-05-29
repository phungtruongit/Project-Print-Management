using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace PrintManagement.AdminApp.Reports {
    public partial class Rpt_PrinterStatistic : DevExpress.XtraReports.UI.XtraReport {
        public Rpt_PrinterStatistic(object datasource) {
            InitializeComponent();
            this.DataSource = datasource;
        }
    }
}
