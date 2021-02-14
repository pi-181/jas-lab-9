using System;
using System.Windows.Forms;

namespace jaslab4
{
    public partial class TripForm : Form
    {
        public MainForm Parent { get; set; }
        public int Id { get; set; }

        public TripForm()
        {
            InitializeComponent();
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            Parent.AddTrip(nameBox.Text, sourceBox.Text, targetBox.Text);
            Parent.FillTripsGrid();
            Visible = false;
            GC.KeepAlive(this);
        }

        private void OnEditButtonClick(object sender, EventArgs e)
        {
            Parent.UpdateTrip(Id, nameBox.Text, sourceBox.Text, targetBox.Text);
            Parent.FillTripsGrid();
            Visible = false;
            GC.KeepAlive(this);
        }

        public void Reset(bool edit, string name, string source, string target)
        {
            editButton.Visible = edit;
            addButton.Visible = !edit;
            nameBox.Text = name;
            sourceBox.Text = source;
            targetBox.Text = target;
        }

    }
}