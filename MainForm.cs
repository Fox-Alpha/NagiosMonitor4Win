/*
 * Erstellt mit SharpDevelop.
 * Benutzer: buck
 * Datum: 08.06.2015
 * Zeit: 13:40
 * 
 */
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
//Für Serialioszierung der Konfigurationen
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
//Für DEBUG
using System.Diagnostics;
//Für Regularexpressions
using System.Text.RegularExpressions;
using NagiosConectionManager;
//Sockets
using System.Net;
using System.Net.Sockets;

namespace NagiosMonitor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		#region Eigenschaften
		int _activeConnectionSet;
		
		public int activeConnectionSet {
			get { return _activeConnectionSet; }
			set { _activeConnectionSet = value; }
		}

		string _strNagiosKonfigFile;
		
		public string strNagiosKonfigFile {
			get { return _strNagiosKonfigFile; }
			set { _strNagiosKonfigFile = value; }
		}

		ArrayList nagiosKonfigList;
		
		//	Menüeintrag für ConnectionManager Formaufruf
		ToolStripMenuItem tsiNagiosConnectionManager;

		Socket s = null;
		IPEndPoint hostEndPoint;
		IPAddress hostAddress = null;
		IPAddress[] IPaddresses = null;
		#endregion
		
		#region Form
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			//
			//	Liste für Eingetragene Verbindungskonfigurationen
			//	Dateinamen festlegen in der die Konfigurationen gespeichert und gelesen werden.
			//
			strNagiosKonfigFile = @"ServerKonfiguration.bin";
			nagiosKonfigList = new ArrayList();
			activeConnectionSet = -1;
			//
			//	####
			//
		}
		void MainForm_Load(object sender, EventArgs e)
		{
			tsiNagiosConnectionManager = new ToolStripMenuItem("Verbindungen Verwaltung", 
			                                    null, 
			                                    verbindungenVerwaltenToolStripMenuItem_Click, 
			                                    "tsiNagiosConnectionManager");
			//Laden der Nagios Konfigurationen und Darstellen im Menü der Statuszeile
			loadKonfigFromFile();	

			//	Setzen der ersten Eintrags als standard Verbindung
			activeConnectionSet	= nagiosKonfigList.Count > 0 ? 0 : -1;

			setConnectionList();
			
			nagiosCreateSocket();
			
			statusTimer.Interval = 5000;
			statusTimer.Start();
		}
		
		void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			//	Stoppen des timers
			if (statusTimer.Enabled) {
				statusTimer.Stop();
			}
			
			//	Schließen des Sockets
			try {
				if(s != null)
				{
					if (s.Connected) {
						s.Disconnect(false);
						s.Shutdown(SocketShutdown.Both);
						s.Close();
					}
					s.Dispose();
				}
				
			} catch (SocketException exSocket) {
				
				Debug.WriteLine("Fehler beim schließen des Sockets: " + exSocket.Message, "MainForm_FormClosing()");
			}
		}
		
		void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
	
		}
		#endregion
		
		#region Funktionen
		#endregion
		
		#region NagiosConnectionManager
		/// <summary>
		/// Laden der gespeicherten Konfigurationen
		/// </summary>
		void loadKonfigFromFile()
		{
			if(!File.Exists(strNagiosKonfigFile))
				return;
			
			using(FileStream fs = new FileStream(strNagiosKonfigFile, FileMode.Open))
			{
			    try 
			    {
			    	BinaryFormatter formatter = new BinaryFormatter();
			    	nagiosKonfigList = (ArrayList)formatter.Deserialize(fs);
			    }
			    catch (SerializationException e) 
			    {
			      // die Datei kann nicht deserialisiert werden
			    	Debug.WriteLine(e.Message, "loadKonfigFromFile().SerializationException");
			    }
			    catch (IOException e)
			    {
			    // Beim Versuch, die Datei zu öffnen, ist ein Fehler aufgetreten.
			    	Debug.WriteLine(e.Message, "loadKonfigFromFile().IOException");
			    }
			    
//			    foreach (object obj in nagiosKonfigList) {
//			    	if(obj != null)
//			    	{
//			    		
//			    	}
//			    		listBox1.Items.Add((obj as NagiosServer).svrConfigName);
//			    }
//			    activeConfigIndex = listBox1.Items.Count > 0 ? 0 : -1;
			}
		}
		
		/// <summary>
		/// Eingelesene Verbindungen als Eintrag hinzufügen
		/// </summary>
		void setConnectionList()
		{
			ToolStripMenuItem tsmiConnect;
			tsddButtConnections.DropDownItems.Add(tsiNagiosConnectionManager);
			
			tsddButtConnections.DropDownItems.Add(new ToolStripSeparator());
			
			foreach (object obj in nagiosKonfigList) {
				tsmiConnect = new ToolStripMenuItem((obj as NagiosServer).svrConfigName, null, setActiveConnectionToolStripMenuItem_Click, "tsiNagiosConnection"+ nagiosKonfigList.IndexOf(obj));
				tsmiConnect.Tag = nagiosKonfigList.IndexOf(obj);
				if (activeConnectionSet == nagiosKonfigList.IndexOf(obj)) {
					tsmiConnect.Checked = true;
				}

				tsddButtConnections.DropDownItems.Add(tsmiConnect);
			}
		}
		
		/// <summary>
		/// Anzeigen der Verbindungsverwaltung aus dem Assembly
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void verbindungenVerwaltenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ConnectionManager ncm = new ConnectionManager();
			
			ncm.strNagiosKonfigfile = strNagiosKonfigFile;
			
			if(ncm.ShowDialog() == DialogResult.OK)
			{
				//Neuladen der Konfigurationsliste
				refreshConnectionList();
			}
			
			ncm.Dispose();
		}
		
		
		/// <summary>
		/// Setzen der zu verwendenden Verbindungskonfiguration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void setActiveConnectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int lastConnectionSet = activeConnectionSet;
			
			activeConnectionSet = Convert.ToInt32((sender as ToolStripMenuItem).Tag);
			
			//	Bei gleicher Auswahl , nichts machen
			if (lastConnectionSet == activeConnectionSet) {
				return;
			}
			
			//	Checked aus vorherigem MenuItem entfernen
			foreach (ToolStripItem tsmi in tsddButtConnections.DropDownItems) {
				if(tsmi as ToolStripMenuItem != null)
					((ToolStripMenuItem)tsmi).Checked = false;
			}
			
			(sender as ToolStripMenuItem).Checked = true;
			
			//	#####
			//	TODO: Socket an neue Auswahl anpassen
			nagiosUpdateSocket();
		}

		/// <summary>
		/// Reload der Verbindungseinträge
		/// Nach dem schließen des Connection Managers
		/// </summary>
		void refreshConnectionList()
		{
			int iLastUsedConnection = -1;
			string strLastUsedConnection = "";
			
			//	Nur wenn eine Connection aktiv war
			if(activeConnectionSet < 0)
				return;
			
			foreach (ToolStripItem tsmi in tsddButtConnections.DropDownItems) {
				if(tsmi as ToolStripMenuItem != null)
				{
					if(((ToolStripMenuItem)tsmi).Checked)
					{
						iLastUsedConnection = Convert.ToInt32((tsmi as ToolStripMenuItem).Tag);
						strLastUsedConnection = ((ToolStripMenuItem)tsmi).Text;
						break;
							//;
					}				
				}
			}			
			
			clearConnectionList();
			loadKonfigFromFile();
			
			//	wiederherstellen der zuletzt gewählten Verbindung
			//	Vergleich mit Index und Name, falls diese sich nach dem Aufruf des ConnectionManagers verändert haben
			//	####
			if (iLastUsedConnection != activeConnectionSet || (nagiosKonfigList[activeConnectionSet] as NagiosServer).svrConfigName != strLastUsedConnection) {
				activeConnectionSet = -1;
				
				foreach (NagiosServer ngs in nagiosKonfigList) {
					if (ngs.svrConfigName == strLastUsedConnection) {
						activeConnectionSet = nagiosKonfigList.IndexOf(ngs);
					}
				}
				activeConnectionSet	= activeConnectionSet < 0 ? 0 : activeConnectionSet;
			}
			//	####
			setConnectionList();
		}

		/// <summary>
		/// Alle Verbindungseinträge im Menü entfernen 
		/// </summary>
		void clearConnectionList()
		{
			if (nagiosKonfigList.Count > 0) 
			{				
				//Bereinigen der Menüliste
				tsddButtConnections.DropDown.Items.Clear();
			}
		}
		#endregion
		
		#region	NagiosFunktionen
		
		/// <summary>
		/// Erzeugt einen globalen Socket
		/// </summary>
		void nagiosCreateSocket(bool newSocket = true)
		{
//			nagiosHost = "HH-LS-IT-007";
//			nagiosLivePort = 6557;
			
			//prüfen ob MKLIve konfiguriert ist
			if (!(nagiosKonfigList[activeConnectionSet]  as NagiosServer).enableLivestatus) {
				MessageBox.Show("In der Verbindung wurde kein MK-Livestatus konfiguriert", "MK-Live deaktiviert");
				return;
			}
			
			// Get DNS host information.
			IPHostEntry hostInfo = Dns.GetHostEntry((nagiosKonfigList[activeConnectionSet]  as NagiosServer).mkliveHostAdress);
			// Get the DNS IP addresses associated with the host.
			IPaddresses = null;
			
			IPaddresses = hostInfo.AddressList;
			

			hostAddress = IPaddresses[0];
			hostEndPoint = null;
			hostEndPoint = new IPEndPoint(hostAddress, (nagiosKonfigList[activeConnectionSet]  as NagiosServer).mklivePort);
			
			//	Schließen des Sockets
			try {
				if(s != null)
				{
					if (s.Connected) {
						s.Disconnect(true);
						s.Shutdown(SocketShutdown.Both);
					}
					s.Close();
//					s.Dispose();
				}
				
			} catch (SocketException exSocket) {
				
				Debug.WriteLine("Fehler beim schließen des Sockets: " + exSocket.Message, "nagiosCreateSocket()");
			}
//			if (newSocket) 
			{
				s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				s.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
				s.ReceiveTimeout = 1500;
			}
		}
		
		/// <summary>
		/// Bei wechsel der Verbindung, den bestehenden Socket 
		/// schließen und einen neuen erzeugen
		/// </summary>
		void nagiosUpdateSocket()
		{
			nagiosCreateSocket(false);
		}
		
		/// <summary>
		/// Abfrage von Nagios nach aktuellen Problem Hosts und Services
		/// </summary>
		void nagiosLiveFullProblemRequest()
		{
			//	####
			//GET hosts
			//Filter: num_services_crit > 0
			//Filter: num_services_unknown > 0
			//Filter: num_services_warn > 0
			//Filter: state > 0
			//Or: 4
			//Columns: name    state services_with_info
			//Separators: 10 59 126 124
			//ResponseHeader: fixed16
			//ColumnHeaders: on
			//	####
			
			string strRequest = 
					"GET hosts\n" + 
					"Filter: num_services_crit > 0\n" +
					"Filter: num_services_unknown > 0\n" +
					"Filter: num_services_warn > 0\n" +
					"Filter: state > 0\n" +
					"Or: 4\n" +
					"Columns: name    state services_with_info\n" +
					"Separators: 10 59 126 124\n" +
					"ResponseHeader: fixed16\n" +
					"ColumnHeaders: on\n";
			
//			string strResult = nagiosDoRequest("GET columns\nFilter: table = hosts\nColumns: name\nResponseHeader: fixed16\n");
			string strResult = nagiosDoRequest(strRequest);
			
			if (!string.IsNullOrWhiteSpace(strResult)) {
				string[] strTemp = nagiosPrepareData(strResult);
				nagiosDisplayGlobalStatus(strTemp, "Hostname;Host Status;Service Name;Service Daten");
			}
		}
		
		/// <summary>
		/// Abfrage aller konfigurierten Hostgruppen
		/// </summary>
		void nagiosRequestHostGroups(TreeNode ParentNode)
		{
			//	####
			//	Alle definierten Hostgruppen
			//	####
			//	GET hostgroups
			//	Columns: name alias
			//	Separators: 10 59 126 124
			//	ResponseHeader: fixed16
			//	ColumnHeaders: on
			//	####
			
			string strRequest =
				"GET hostgroups\n" +
				"Columns: name alias num_hosts num_hosts_up\n" +
				"Separators: 10 59 126 124\n" +
				"ResponseHeader: fixed16\n"; // + "ColumnHeaders: on\n";
			
			string strResult = nagiosDoRequest(strRequest);
			int iNodeIndex = -1;
			
			if (!string.IsNullOrWhiteSpace(strResult)) {
				string[] strTemp = nagiosPrepareData(strResult);
				
				if (ParentNode != null && strTemp.Length > 0) {
					if (ParentNode.Nodes.Count > 0) {
						ParentNode.Nodes.Clear();
					}
					int iNodeCount = 0;
					int iHostCount = 0, iHostUp = 0;
					bool doNotChangeState = false;
					
					foreach (string element in strTemp) 
					{
						if (!string.IsNullOrWhiteSpace(element)) 
						{
							string[] strInfo = element.Split(new char[] {';'});
							iNodeIndex = ParentNode.Nodes.Add(new TreeNode() {ToolTipText=strInfo[1], Text = strInfo[0], Name = "hg" + ++iNodeCount});
	
							//	####
							//	Status ermitteln
							//	####
							if (strInfo.Length == 4) {
								Int32.TryParse(strInfo[2], out iHostCount);
	//								iSvcCount = strInfo[2];
								
								Int32.TryParse(strInfo[3], out iHostUp);
	//								iSvcOk = strInfo[3];

								if (iHostCount == iHostUp) {
									if (!doNotChangeState) {
										ParentNode.BackColor = Color.Green;
									}
									ParentNode.Nodes[iNodeIndex].BackColor = Color.Green;
								}
								else
								{
									ParentNode.BackColor = Color.Red;
									ParentNode.Nodes[iNodeIndex].BackColor = Color.Red;
									doNotChangeState = true;
								}
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Abfrage aller Konfigurierten Servicegruppen
		/// </summary>
		void nagiosRequestServiceGroups(TreeNode ParentNode)
		{
			//	####
			//	Abfrage aller Konfigurierten Servicegruppen
			//	####
			//	GET servicegroups
			//	Columns: name
			//	####
			string strRequest =
				"GET servicegroups\n" +
				"Columns: name alias num_services num_services_ok num_services_warn num_services_crit num_services_unknown\n" +
				"Separators: 10 59 126 124\n" +
				"ResponseHeader: fixed16\n"; // + "ColumnHeaders: on\n";
			
			string strResult = nagiosDoRequest(strRequest);
			
			if (!string.IsNullOrWhiteSpace(strResult)) {
				string[] strTemp = nagiosPrepareData(strResult);
				
				if (ParentNode != null && strTemp.Length > 0) {
					if (ParentNode.Nodes.Count > 0) {
						ParentNode.Nodes.Clear();
					}
					
					int iNodeCount = 0;
					int iNodeIndex = -1;
					int iSvcCount = 0, iSvcOk = 0, iSvcWarn = 0, iSvcCrit = 0, iSvcUnkn = 0;
					bool doNotChangeState = false;
					
					foreach (string element in strTemp) {
						if (!string.IsNullOrWhiteSpace(element)) {
							
							string[] strInfo = element.Split(new char[] {';'});
							iNodeIndex = ParentNode.Nodes.Add(new TreeNode() {ToolTipText=strInfo[1], Text = strInfo[0], Name = "sg" + ++iNodeCount});
							
							//	####
							//	Status ermitteln
							//	####
							if (strInfo.Length == 7) {
								Int32.TryParse(strInfo[2], out iSvcCount);
//								iSvcCount = strInfo[2];
								
								Int32.TryParse(strInfo[3], out iSvcOk);
//								iSvcOk = strInfo[3];
								
								Int32.TryParse(strInfo[4], out iSvcWarn);
//								iSvcWarn = strInfo[4];
								
								Int32.TryParse(strInfo[5], out iSvcCrit);
//								iSvcCrit = strInfo[5];
								
								Int32.TryParse(strInfo[6], out iSvcUnkn);
//								iSvcUnkn = strInfo[6];
								
								if (iSvcCount == iSvcOk) {
									if (!doNotChangeState) {
										ParentNode.BackColor = Color.Green;
									}
									ParentNode.Nodes[iNodeIndex].BackColor = Color.Green;
								}
								else
								{
									if(iSvcWarn > 0)
									{
										ParentNode.BackColor = Color.Yellow;
										ParentNode.Nodes[iNodeIndex].BackColor = Color.Yellow;
										doNotChangeState = true;
									}
									if(iSvcUnkn > 0)
									{
										ParentNode.BackColor = Color.CadetBlue;
										ParentNode.Nodes[iNodeIndex].BackColor = Color.CadetBlue;
										doNotChangeState = true;
									}
									if(iSvcCrit > 0)
									{
										ParentNode.BackColor = Color.Red;
										ParentNode.Nodes[iNodeIndex].BackColor = Color.Red;
										doNotChangeState = true;
									}
								}
							}
							//	####
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Abfrage der ausgewählten Hostgruppe
		/// Details zu allen Systemen
		/// </summary>
		/// <param name="hostGroupName">Name der Hostgruppe die Abgefragt werden soll</param>
		/// <param name="ParentNode">Node im TreeView dem die Daten zugeordnet werden sollen</param>
		void nagiosRequestHostgroupDetails(string hostGroupName, TreeNode ParentNode)
		{
			//	####
			//	Liste mit zugehörigen Servern
			//		- Jedes System als Unterknoten auflisten
			//	####
			//	GET hostgroups
			//	Columns: name alias members
			//	Separators: 10 59 126 124
			//	ResponseHeader: fixed16
			//	ColumnHeaders: on
			//	####
			string strRequest =
				"GET hostgroups\n" +
				"Columns:  members_with_state\n" + //name alias 
				string.Format("Filter: name = {0}\n", hostGroupName) +
				"Separators: 10 59 126 124\n" +		//	TODO: Manuelle Angaben aus Optionen übernehmen
				"ResponseHeader: fixed16\n" +
				"ColumnHeaders: on\n";
			
			string strResult = nagiosDoRequest(strRequest);
			int iNodeIndex = -1;
			
			if (!string.IsNullOrWhiteSpace(strResult)) {
				string[] strTemp = nagiosPrepareData(strResult);
				
				if (ParentNode != null && strTemp.Length >= 3) {
					if (ParentNode.Nodes.Count > 0) {
						ParentNode.Nodes.Clear();
					}
					int iNodeCount = 0;
					string[] members = strTemp[2].Split(new char[] {'~'});
					foreach (string element in members) {
						if (!string.IsNullOrWhiteSpace(element)) {
							
							bool doNotChangeState = false;
							string strState = element.Split(new char[] {'|'})[1];
							int iState = 0;
							int iNodeChildCount = -1;
							
							
							iNodeIndex = ParentNode.Nodes.Add(new TreeNode() {ToolTipText = element.Split(new char[] {'|'})[0], Text = element.Split(new char[] {'|'})[0], Name = "hgmember" + ++iNodeCount});
							
							//	Zugehörige Hosts eintragen
//							string hosts = element.Split(new char[] {'|'})[1];
//							int iChildNodeIndex = -1;
//							if (!string.IsNullOrWhiteSpace(hosts)) {
//								iChildNodeIndex = ParentNode.Nodes[iNodeIndex].Nodes.Add(new TreeNode() {ToolTipText = hosts, Text = hosts, Name = "hgmemberchild" + ++iNodeChildCount});
//							}
							//	####
							if (iNodeChildCount > 0) {
								ParentNode.Nodes[iNodeIndex].Expand();
							}
							//	####
							//	Status farblich markieren
							//	####
							if (Int32.TryParse(strState, out iState)) {
								switch (iState) 
								{
									case 0:		//	OK
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.Green;
											ParentNode.BackColor = Color.Green;
										}
										break;
									case 1:		//	Warn
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.Yellow;
											ParentNode.BackColor = Color.Yellow;
										}
										doNotChangeState = true;
										break;
									case 2:		//	Critical
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.Red;
											ParentNode.BackColor = Color.Red;
										}
										doNotChangeState = true;
										break;
									case 3:		//	Unknown
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.CadetBlue;
											ParentNode.BackColor = Color.CadetBlue;
										}
										doNotChangeState = true;
										break;
								}
							}
							//	####
//							if (Int32.TryParse(strState, out iState)) {
//								switch (iState) 
//								{
//									case 0:
//										ParentNode.Nodes[iNodeIndex].BackColor = Color.Green;
//										break;
//										//Warning, Unknown, Critical
//									case 1:	
//									case 2:
//									case 3:
//										ParentNode.Nodes[iNodeIndex].BackColor = Color.Red;
//										break;
//									
//								}
//							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Abfrage der ausgewählten Servicegruppe
		/// Details zu allen Services
		/// </summary>
		/// <param name="serviceGroupName"></param>
		/// <param name="ParentNode">Node im TreeView dem die Daten zugeordnet werden sollen</param>
		void nagiosRequestServicegroupDetails(string serviceGroupName, TreeNode ParentNode)
		{
			//	####
			//	Inkl. Hostnamen und zugeordnete Services
			//	####
			//	GET servicegroups
			//	Columns: name members_with_state
			//	####
			Hashtable htServiceGroup;
			
			htServiceGroup = new Hashtable();
			
			
			string strRequest =
				"GET servicegroups\n" +
				"Columns: members_with_state\n" + //name alias 
				string.Format("Filter: name = {0}\n", serviceGroupName) +
				"Separators: 10 59 126 124\n" +		//	TODO: Manuelle Angaben aus Optionen übernehmen
				"ResponseHeader: fixed16\n" +
				"ColumnHeaders: on\n";
			
			string strResult = nagiosDoRequest(strRequest);
			int iNodeIndex = -1;
			
			if (!string.IsNullOrWhiteSpace(strResult)) {
				string[] strTemp = nagiosPrepareData(strResult);
				
				if (ParentNode != null && strTemp.Length >= 3) {
					if (ParentNode.Nodes.Count > 0) {
						ParentNode.Nodes.Clear();
					}
					int iNodeCount = 0;
					int iNodeChildCount = 0;
					int iSvcCount = 1;
					
					string[] members = strTemp[2].Split(new char[] {'~'});
					
					//	####
					//	Darstellen der Servicegruppe
					//	####
					//	Gruppenname
					//	|-	Host in Gruppe
					//	|	|-	Services des Host mit Status
					//	####
					foreach (string element in members) {
						if (!string.IsNullOrWhiteSpace(element)) {
							
							string strState = element.Split(new char[] {'|'})[2];
							string strSvcName = element.Split(new char[] {'|'})[1];
							string strSvcMemberHost = element.Split(new char[] {'|'})[0];
							int iState = 0;
							bool doNotChangeState = false;
							
							
							//	####
							//	Prüfen ob Host bereits vorhanden
							//	####
							if (!htServiceGroup.ContainsKey(strSvcMemberHost)) {
								iNodeIndex = ParentNode.Nodes.Add(new TreeNode() {ToolTipText = element.Split(new char[] {'|'})[0], Text = element.Split(new char[] {'|'})[0], Name = "sgmember" + ++iNodeCount});
								htServiceGroup.Add(strSvcMemberHost, iNodeIndex);
							}
							//
							//	Wenn ja, dann Index holen
							//
							else
							{
//								htServiceGroup[strSvcName] = ++iSvcCount;
								iNodeIndex = (int)htServiceGroup[strSvcMemberHost];
								
							}
							//	####

							//	Zugehörige Hosts eintragen
							string hosts = element.Split(new char[] {'|'})[1];
							int iChildNodeIndex = -1;
							if (!string.IsNullOrWhiteSpace(hosts)) {
								iChildNodeIndex = ParentNode.Nodes[iNodeIndex].Nodes.Add(new TreeNode() {ToolTipText = hosts, Text = hosts, Name = "sgmemberchild" + ++iNodeChildCount});
							}
							//	####
							
							if (iNodeChildCount > 0) {
								ParentNode.Nodes[iNodeIndex].Expand();
							}
							//	####
							//	Status farblich markieren
							//	####
							if (Int32.TryParse(strState, out iState)) {
								switch (iState) 
								{
									case 0:		//	OK
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.Green;
											ParentNode.Nodes[iNodeIndex].Nodes[iChildNodeIndex].BackColor = Color.Green;
										}
										break;
									case 1:		//	Warn
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.Yellow;
											ParentNode.Nodes[iNodeIndex].Nodes[iChildNodeIndex].BackColor = Color.Yellow;
										}
										doNotChangeState = true;
										break;
									case 2:		//	Critical
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.Red;
											ParentNode.Nodes[iNodeIndex].Nodes[iChildNodeIndex].BackColor = Color.Red;
										}
										doNotChangeState = true;
										break;
									case 3:		//	Unknown
										if (!doNotChangeState) {
											ParentNode.Nodes[iNodeIndex].BackColor = Color.CadetBlue;
											ParentNode.Nodes[iNodeIndex].Nodes[iChildNodeIndex].BackColor = Color.CadetBlue;
										}
										doNotChangeState = true;
										break;
								}
							}
							//	####
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Abfrage per MK-Livestatus ausführen
		/// </summary>
		/// <param name="strRequest"></param>
		string nagiosDoRequest(string strRequest)
		{
			Encoding ASCII = Encoding.ASCII;
			Byte[] ByteGet = ASCII.GetBytes(strRequest);
			Byte[] RecvBytes = new Byte[256];
			String strRetPage = null;
			
			try {
				hostAddress = IPaddresses[0];
//				hostEndPoint = new IPEndPoint(hostAddress, nagiosLivePort);
				hostEndPoint = new IPEndPoint(hostAddress, 6557);
				
				s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				s.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
				
				// Connect to the host using its IPEndPoint.
				if (!s.Connected) {
					s.Connect(hostEndPoint);
				}				
				
				// Sent the GET request to the host.
				Int32 bytesend = s.Send(ByteGet, ByteGet.Length, SocketFlags.None);
				
//				textBox1.Text = GET;

				if (s != null) {
					s.Shutdown(SocketShutdown.Send);
				}
				// Receive the host home page content and loop until all the data is received.
				Int32 bytes = s.Receive(RecvBytes, RecvBytes.Length, SocketFlags.None);
//				Int32 bytes = s.Receive(RecvBytes, s.Available, SocketFlags.None);

				while (bytes > 0) {
					strRetPage = strRetPage + ASCII.GetString(RecvBytes, 0, bytes);
					bytes = s.Receive(RecvBytes, RecvBytes.Length, 0);
				}
				
			} catch (SocketException exc) {
				Debug.WriteLine("SocketException caught!!!");
				Debug.WriteLine("Source : " + exc.Source);
				Debug.WriteLine("Message : " + exc.Message);
				Debug.WriteLine("SocketErrorCode : " + exc.SocketErrorCode);
			} catch (ArgumentNullException exc) {
				Debug.WriteLine("ArgumentNullException caught!!!");
				Debug.WriteLine("Source : " + exc.Source);
				Debug.WriteLine("Message : " + exc.Message);
			} catch (NullReferenceException exc) {
				Debug.WriteLine("NullReferenceException caught!!!");
				Debug.WriteLine("Source : " + exc.Source);
				Debug.WriteLine("Message : " + exc.Message);
			} catch (Exception exc) {
				Debug.WriteLine("Exception caught!!!");
				Debug.WriteLine("Source : " + exc.Source);
				Debug.WriteLine("Message : " + exc.Message);
			}
			
			if (s != null) {
//				s.Disconnect(true);
//				s.Shutdown(SocketShutdown.Both);
				s.Close();
			}
//
			//	Rückgabe der empfangenen Rohdaten
			rtbNagiosDebug.Text = strRetPage;
			return strRetPage;
			
		}
		
		string[] nagiosPrepareData(string strData)
		{
			char[] charSeparators = new char[] {'\n'};
			string[] strZeilen;
			int iStat = 0;
			strZeilen = strData.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
			
			//	Status auf Fehler prüfen, 200 = Abfrage OK
			if((iStat = nagiosRequestStatus(strZeilen[0]))== 200)
			{
				//Weiterverarbeiten
				Debug.WriteLine(strZeilen[0], "nagiosPrepareData()");
			}
			else if(iStat < 0)	//	Es wurde eine Exception ausgelöst, oder MK-Livestatus gab einen unbekannten Fehler zurück
				Debug.WriteLine("Exception aufgetreten", "nagiosPrepareData()");
			
			//	Entfernt Statuszeile aus den Daten
			if (strZeilen.Length != 0) {
				strZeilen[0] = null;
			}
			
			return strZeilen;
		}

		/// <summary>
		/// Ermittelt den Status der Abfrage
		/// </summary>
		/// <param name="strData"></param>
		/// <returns></returns>
		int nagiosRequestStatus(string strData)
		{
			int nagStat = 0;
			int nagDataLenght = 0;
			char[] trimChar = {' ', '\n'};
			string strNagStat = null;
			try 
			{
				string strTemp = null;
				
				//Statuscode
				strTemp = strData.Substring(0,3);
				nagStat = Convert.ToInt32(strTemp);	
				
				//Länge der Daten
				strTemp = strData.Substring(3,strData.Length-3).Trim(trimChar);
				nagDataLenght = Convert.ToInt32(strTemp);
			} 
			catch (NullReferenceException nre) 
			{
				Debug.WriteLine("NullReferenceException caught!!!");
				Debug.WriteLine("Source : " + nre.Source);
				Debug.WriteLine("Message : " + nre.Message);
				return -1;
			}
			catch(Exception exc)
			{
				Debug.WriteLine("GeneralException caught!!!");
				Debug.WriteLine("Source : " + exc.Source);
				Debug.WriteLine("Message : " + exc.Message);
				return -1;
			}
			
			
			switch(nagStat)
			{
				case 200:
					//200	OK. Reponse contains the queried data.
					//Daten aufbereiten
					//processNagiosData(strRetPage.Substring(16, nagDataLenght));
					strNagStat = "Status: 200\tOK.";
					break;
					//400	The request contains an invalid header.
				case 400:
					strNagStat = "Fehler: 400\tThe request contains an invalid header";
					break;
					//403	The user is not authorized (see AuthHeader)
				case 403:
					strNagStat = "Fehler: 403\tThe user is not authorized (see AuthHeader)";
					break;
					//404	The target of the GET has not been found (e.g. the table).
				case 404:
					strNagStat = "Fehler: 404\tThe target of the GET has not been found (e.g. the table).";
					break;
					//450	A non-existing column was being referred to
				case 450:
					strNagStat = "Fehler: 450\tA non-existing column was being referred to";
					break;
					//451	The request is incomplete.
				case 451:
					strNagStat = "Fehler: 451\tThe request is incomplete.";
					break;
					//452	The request is completely invalid.
				case 452:
					strNagStat = "Fehler: 452\tThe request is completely invalid.";
					Debug.WriteLine("Bei der Abfrage ist ein Fehler aufgetreten({1}): {0}", nagStat, Text);
					//TODO: Fehler per Delegate im ListView anzeigen
					//lvNagiosStatus.Items.Add(string.Format("Fehler {0}",nagStat), strRetPage.Substring(16, strRetPage.Length));
					break;
				
				default:
					nagStat = -99;
					Debug.WriteLine("Nagios Status Abfrage, Unbekannter Status oder Fehler aufgetreten {0}", nagStat);	
					//return false;
					break;
			}
			
			//Anzeigen in Statuszeile
			toolStripStatusLabel1.Text = string.Format("Status der Abfrage: {0} || Empfange Bytes: {1}", strNagStat, nagDataLenght);
			
			return nagStat;
		}
		
		void nagiosDisplayGlobalStatus(string[] strData, string useHeader = null)
		{
			string strHeader = "";
			if (strData.Length > 0) {
				
				if (!string.IsNullOrWhiteSpace(useHeader)) {
					strHeader = useHeader;
				}
				else
					strHeader = strData[1];
				
				
				strData[1] = null;
				
				ListView.ListViewItemCollection lviHostStatusList = new ListView.ListViewItemCollection(lvGlobalNagiosStatus);
				
				ListViewItem lviTemp;
				
				//	####
				//	Spaltenüberschriften erstellen
				//	####
				string[] arrHeader = strHeader.Split(new char[] {';'}, StringSplitOptions.None);
				
				lvGlobalNagiosStatus.Items.Clear();
				lvGlobalNagiosStatus.Columns.Clear();
				
				//	Überschriften setzen
				foreach (string Header in arrHeader) {
					lvGlobalNagiosStatus.Columns.Add(Header);
				}
				//	####
				
				
				//	####
				//	Daten in Spalten splitten
				//	####
				
				foreach (string strDataColumn in strData) {
					if (!string.IsNullOrWhiteSpace(strDataColumn)) {
						Color stateBackCol = Color.Green;
						string[] strHostData = strDataColumn.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
						//Hostname
						lviTemp = new ListViewItem(strHostData[0]);
						//	####
						//	HostStatus
						//	####
						int iState = -1;
						if(Int32.TryParse(strHostData[1], out iState))
						{
							switch (iState) 
							{
								case 0:		//OK
									lviTemp.SubItems.Add("OK");
									break;
								case 1:		//Warn
//									lviTemp.SubItems.Add();
//									lviTemp.BackColor = Color.Yellow;
//									break;
								case 2:		//Unknown
//									lviTemp.SubItems.Add();
//									lviTemp.BackColor = Color.CadetBlue;
//									break;
								case 3:		//Down
									lviTemp.SubItems.Add("CRITICAL");
									break;
							}
							lviTemp.BackColor = stateBackCol;
							lviTemp.SubItems[1].BackColor = stateBackCol;
						}
						//	####
						
						//	Services und deren Status
						string[] strHostServices = strHostData[2].Split(new char[] {'~'}, StringSplitOptions.RemoveEmptyEntries);
						
						foreach (string strHostSvcData in strHostServices) {
							string[] strHostServiceData = strHostSvcData.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
							
							//	####
							//	Servicestatus
							//	####
							iState = 0;
							if (Int32.TryParse(strHostServiceData[1], out iState)) {
								switch (iState) 
								{
									case 0:		//	OK
										stateBackCol = Color.Green;
										break;
									case 1:		//	Warn
										stateBackCol = Color.Yellow;
										break;
									case 2:		//	Critical
										stateBackCol = Color.Red;
										break;
									case 3:		//	Unknown
										stateBackCol = Color.CadetBlue;
										break;
								}
							}
							//	####
							
							if (lviHostStatusList.Count > 0) {
								lviTemp = new ListViewItem(new string[] {"", "", strHostServiceData[0], strHostServiceData[3]});
							}
							else
							{
								lviTemp.SubItems.Add(strHostServiceData[0]);
								lviTemp.SubItems.Add(strHostServiceData[3]);
							}
							
							lviTemp.SubItems[2].BackColor= stateBackCol;
							lviTemp.SubItems[3].BackColor= stateBackCol;
							
							lviTemp.UseItemStyleForSubItems = false;
							
							lviHostStatusList.Add(lviTemp);
						}
					}
				}
				lvGlobalNagiosStatus.BeginUpdate();
				lvGlobalNagiosStatus.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				lvGlobalNagiosStatus.EndUpdate();
				//	####
			}
		}
		#endregion
		
		#region Events
		
		/// <summary>
		/// Timer Event zum intervall abfragen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void statusTimer_Tick(object sender, EventArgs e)
		{
			//	Timer zur intervall Abfrage 
			//	Default 30 Sek., disabled
			//	Wird vom Nagioswert überschrieben
			//	####
			//	Ruft die Abfrage der aktuellen selektierten Gruppe oder selektierte Statusdetails ab
			//	####
			nagiosLiveFullProblemRequest();
			//	####
		}
		
		#region TreeView
		
		/// <summary>
		/// Vor dem aufklappen des Baumes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void tvNagiosGroups_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			// 	Vor dem aufklappen des Baumes
			//	Abfragen der Gruppen im Knoten
		}
		
		/// <summary>
		/// Nach dem Selectieren eines Knotens
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void tvNagiosGroups_AfterSelect(object sender, TreeViewEventArgs e)
		{
			//	Statusabfrage der gewählten Gruppe
			if (e.Action != TreeViewAction.Unknown)
	        {
                //TODO:
                //	- Prüfen welche Node gewählt wurde
                //	- Livestatus abfragen
                //	-- Hosts, Services, -gruppen, etc.
                //	- ChildNodes erzeugen
                //	- Informationen anzeigen
                
                //	Node der obersten Ebene
                if (e.Node.Parent == null) {
                	int iNode;
                	
                	if (Int32.TryParse(e.Node.Tag as string, out iNode)) {
                		iNode = Convert.ToInt32(e.Node.Tag);
                		
                		switch (iNode) 
                		{
                			case 1:		//	Alle Probleme Übersicht
                				
                				break;
                			
                			case 200:	//	Hostgruppen
                				nagiosRequestHostGroups(e.Node);
                				break;
                			
                			case 500:	//	Servicegruppen
                				nagiosRequestServiceGroups(e.Node);
                				break;
                			
                		}
                	}
                	
                	//	####
                	//	Alle Nodes schließen
                	(sender as TreeView).CollapseAll();
                	//	Nur aktuellen Node entfalten
                	e.Node.Expand();
                	//	####
                }
                else
                {
                	//	Node ist Child
                	//	TODO: 
                	//	Feststellen welcher Node Parent ist
                	//	Zweck: Gruppe, Host oder Service Eintrag
                	//	Abhängig von Zweck eine Abfrage tätigen
                	//	Empfangene Daten aufbereiten und anzeigen
                	//	Eventl. Childnodes erstellen (Gruppen)
                	
                	string strNodegroup = e.Node.Text;
                	
                	//	Prüfen ob es sich um einen Member Eintrag handelt
                	if (Regex.IsMatch(e.Node.Name, "^(hg|sg)member[0-9]{1,5}")) {
                		
                	}
                	
                	//	Prüfen ob es sich um eine Hostgruppe Eintrag handelt
                	if (Regex.IsMatch(e.Node.Name, "^hg[0-9]{1,5}")) {
                		nagiosRequestHostgroupDetails(strNodegroup, e.Node);
                	}
                	
                	//	Prüfen ob es sich um eine Servicegruppe Eintrag handelt
					if (Regex.IsMatch(e.Node.Name, "^sg[0-9]{1,5}")) {
                		nagiosRequestServicegroupDetails(strNodegroup, e.Node);
                	}

                	(sender as TreeView).CollapseAll();
                	
                	TreeNode tnTemp = e.Node;
                	TreeNode tnParent = null;
                	while((tnParent = tnTemp.Parent)!=null)
                	{
                		tnParent.Expand();
                		tnTemp = tnParent;
                	}
//                	e.Node.Parent.Expand();
                	e.Node.Expand();
                	(sender as TreeView).SelectedNode = e.Node;
                	//	Anzeigen der Daten im ListView
                }
                
                
                
	            if (e.Node.IsExpanded) 
	            {
//	                e.Node.Collapse();
	            }
	            else 
	            {
//	                e.Node.Expand();
	            }
	        }
	
	        // Remove the selection. This allows the same node to be
	        // clicked twice in succession to toggle the expansion state.
//	        (sender as TreeView).SelectedNode = null;
		}
		#endregion
		
		#endregion
	}
}
