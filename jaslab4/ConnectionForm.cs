using System;
using System.Reflection;
using System.Windows.Forms;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace jaslab4
{
    public partial class ConnectionForm : Form
    {
        private ISessionFactory factory;
        private ISession session;

        public MainForm Parent { get; set; }

        public ConnectionForm()
        {
            InitializeComponent();
        }

        private void OnConnectButtonClick(object sender, EventArgs e)
        {
            session = OpenSession(hostBox.Text, int.Parse(portBox.Text), databaseBox.Text, userBox.Text, passwordBox.Text);
            
            Visible = false;
            Parent.Session = session;
            
            Parent.FillCabinsGrid();
            Visible = false;
        }

        private ISession OpenSession(string host, int port, string database, string user, string passwd)
        {
            if (factory == null)
            {
                factory = Fluently.Configure()
                    .Database(PostgreSQLConfiguration
                        .PostgreSQL82.ConnectionString(c => c
                            .Host(host)
                            .Port(port)
                            .Database(database)
                            .Username(user)
                            .Password(passwd)
                        )
                    )
                    .Mappings(m => m.FluentMappings.Add<CabinMap>().Add<PassengerMap>())
                    .ExposeConfiguration(BuildSchema)
                    .BuildSessionFactory();
            }

            return factory.OpenSession();
        }
        
        private static void BuildSchema(Configuration config)
        {
            new SchemaExport(config).Create(false, true);
        }
    }
}