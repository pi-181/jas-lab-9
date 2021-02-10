using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NHibernate;
using Npgsql;

namespace jaslab4
{
    public partial class MainForm : Form
    {
        public ISession Session { get; set; }

        private readonly ConnectionForm _connectionForm;
        private readonly CabinForm _cabinForm;
        private readonly PassengerForm _passengerForm;

        private int _selectedCabinId = -1;
        
        public MainForm()
        {
            InitializeComponent();

            cabinGrid.Columns.Add("cabin_name", "cabin_name");
            cabinGrid.Columns.Add("square", "square");
            cabinGrid.Columns.Add("class_name", "class_name");
            
            passengerGrid.Columns.Add("first_name", "first_name");
            passengerGrid.Columns.Add("last_name", "last_name");
            passengerGrid.Columns.Add("sex", "sex");
            OnSplitterMoved(null, null);

            _connectionForm = new ConnectionForm {Parent = this};
            _cabinForm = new CabinForm {Parent = this};
            _passengerForm = new PassengerForm {Parent = this};
        }

        public void AddCabin(string name, int square, string className)
        {
            new NHibernateDAOFactory(Session).getCabinDAO().SaveOrUpdate(new Cabin
            {
                CabinName = name,
                Square = square,
                ClassName = className
            });
        }

        public void UpdateCabin(int id, string name, int square, string className)
        {
            var cabinDao = new NHibernateDAOFactory(Session).getCabinDAO();
            var byId = cabinDao.GetById(id);
            byId.CabinName = name;
            byId.Square = square;
            byId.ClassName = className;
            cabinDao.SaveOrUpdate(byId);
        }

        public void AddPassenger(string firstName, string lastName, string sex, int cabinId)
        {
            var factory = new NHibernateDAOFactory(Session);
            factory.getPassengerDAO().SaveOrUpdate(new Passenger
            {
                FirstName = firstName,
                LastName = lastName,
                Sex = sex,
                Cabin = factory.getCabinDAO().GetById(cabinId)
            });
        }

        public void UpdatePassenger(int id, string firstName, string lastName, string sex)
        {
            var passengerDao = new NHibernateDAOFactory(Session).getPassengerDAO();
            var byId = passengerDao.GetById(id);
            byId.FirstName = firstName;
            byId.LastName = lastName;
            byId.Sex = sex;
            passengerDao.SaveOrUpdate(byId);
        }

        public void FillCabinsGrid()
        {
            cabinGrid.Rows.Clear();

            var cabinDao = new NHibernateDAOFactory(Session).getCabinDAO();
            var cabins = cabinDao.GetAll();

            foreach (var c in cabins)
            {
                var row = new DataGridViewRow {Tag = c};
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = c.CabinName});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(int), Value = c.Square});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = c.ClassName});
                cabinGrid.Rows.Add(row);
            }
        }

        public void FillPassengersGrid(int cabinId)
        {
            passengerGrid.Rows.Clear();
            
            var passengerDao = new NHibernateDAOFactory(Session).getPassengerDAO();
            var passengers = passengerDao.GetPassengerByCabin(cabinId);
            foreach (var p in passengers)
            {
                var row = new DataGridViewRow {Tag = p};
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = p.FirstName});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = p.LastName});
                row.Cells.Add(new DataGridViewTextBoxCell {ValueType = typeof(string), Value = p.Sex});
                passengerGrid.Rows.Add(row);
            }

        }

        private void OnConnectItemClick(object sender, EventArgs e)
        {
            if (Session != null) OnDisconnectItemClick(sender, e);
            _connectionForm.Visible = true;
        }

        private void OnDisconnectItemClick(object sender, EventArgs e)
        {
            Session = null;
            _selectedCabinId = -1;
            cabinGrid.Rows.Clear();
            passengerGrid.Rows.Clear();
        }

        private void OnContextCabinAddClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            _cabinForm.Id = 0;
            _cabinForm.Visible = true;
            _cabinForm.Reset(false, "", 0, "");
        }

        private void OnContextCabinRemoveClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selected = cabinGrid.SelectedRows;
            if (selected.Count == 0) return;
            
            var cabin = (Cabin) selected[0].Tag;
            var dr = MessageBox.Show("Видалити каюту?", "", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;
            
            new NHibernateDAOFactory(Session).getCabinDAO().Delete(cabin);
            FillCabinsGrid();
        }

        private void OnContextCabinEditClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selectedRows = cabinGrid.SelectedRows;
            if (selectedRows.Count == 0) return;
            
            var cabin = (Cabin) selectedRows[0].Tag;
            _cabinForm.Reset(true, cabin.CabinName, cabin.Square, cabin.ClassName);
            _cabinForm.Id = cabin.Id;
            _cabinForm.Visible = true;
        }

        private void OnContextPassengerAddClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            if (_selectedCabinId == -1) return;

            _passengerForm.Reset(false, "", "", "");
            _passengerForm.Id = 0;
            _passengerForm.CabinId = _selectedCabinId;
            _passengerForm.Visible = true;
        }

        private void OnContextPassengerRemoveClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selected = passengerGrid.SelectedRows;
            if (selected.Count == 0) return;

            var dr = MessageBox.Show("Видалити пасажира?", "", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            var passenger = (Passenger) selected[0].Tag;
            var cabinId = passenger.Cabin.Id;
            
            new NHibernateDAOFactory(Session).getPassengerDAO().Delete(passenger);
            FillPassengersGrid(cabinId);
        }

        private void OnContextPassengerEditClick(object sender, EventArgs e)
        {
            if (Session == null) return;
            var selectedRows = passengerGrid.SelectedRows;
            if (selectedRows.Count == 0) return;
            
            var p = (Passenger) selectedRows[0].Tag;
            _passengerForm.Reset(true, p.FirstName, p.LastName, p.Sex);
            _passengerForm.Id = p.Id;
            _passengerForm.CabinId = _selectedCabinId;
            _passengerForm.Visible = true;
        }

        private void OnCabinCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Session == null) return;
            var sel = cabinGrid.SelectedRows;
            if (sel.Count == 0) return;

            var cabin = (Cabin) sel[0].Tag;
            FillPassengersGrid(_selectedCabinId = cabin.Id);
        }
        
        private void OnSplitterMoved(object sender, SplitterEventArgs e)
        {
            cabinGrid.Columns["cabin_name"].Width = cabinGrid.Width / 3 - 1;
            cabinGrid.Columns["square"].Width = cabinGrid.Width / 3 - 1;
            cabinGrid.Columns["class_name"].Width = cabinGrid.Width / 3 - 1;
            passengerGrid.Columns["first_name"].Width = passengerGrid.Width / 3 - 1;
            passengerGrid.Columns["last_name"].Width = passengerGrid.Width / 3 - 1;
            passengerGrid.Columns["sex"].Width = passengerGrid.Width / 3 - 1;
        }
    }
}