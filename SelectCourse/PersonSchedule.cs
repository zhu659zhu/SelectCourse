using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SelectCourse
{
    public partial class PersonSchedule : Form
    {
        string str;
        public PersonSchedule(string textValue)
        {
            InitializeComponent();
            this.str = textValue;
        }

        private void PersonSchedule_Load(object sender, EventArgs e)
        {
            WebBrowser w = new WebBrowser();
            w.Parent = this;
            w.Dock = DockStyle.Fill;

            w.DocumentText = str;
        }
    }
}
