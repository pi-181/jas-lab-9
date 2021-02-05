using System;
using System.Windows.Forms;

namespace jaslab4
{
    public partial class CabinForm : Form
    {
        public MainForm Parent { get; set; }
        public int Id { get; set; }

        public CabinForm()
        {
            InitializeComponent();
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            Parent.AddCabin(nameBox.Text, int.Parse(squareBox.Text), classBox.Text);
            Parent.FillCabinsGrid();
            Visible = false;
        }

        private void OnEditButtonClick(object sender, EventArgs e)
        {
            Parent.UpdateCabin(Id, nameBox.Text, int.Parse(squareBox.Text), classBox.Text);
            Parent.FillCabinsGrid();
            Visible = false;
        }

        public void Reset(bool edit, string name, int square, string className)
        {
            editButton.Visible = edit;
            addButton.Visible = !edit;
            nameBox.Text = name;
            squareBox.Text = square.ToString();
            classBox.Text = className;
        }

    }
}