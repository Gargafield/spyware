namespace Client.Windows;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.usernameLabel = new Label();
        this.usernameTextBox = new TextBox();
        this.passwordLabel = new Label();
        this.passwordTextBox = new TextBox();
        this.loginButton = new Button();
        this.loginPanel = new FlowLayoutPanel();

        this.dashboardLabel = new Label();
        this.logoutButton = new Button();
        this.raiseHandButton = new Button();
        this.dashboardPanel = new FlowLayoutPanel();

        this.SuspendLayout();

        // usernameLabel
        this.usernameLabel.AutoSize = true;
        this.usernameLabel.Name = "usernameLabel";
        this.usernameLabel.Text = "Username";
        // usernameTextBox
        this.usernameTextBox.Name = "usernameTextBox";
        this.usernameTextBox.Size = new System.Drawing.Size(190, 23);
        // passwordLabel
        this.passwordLabel.AutoSize = true;
        this.passwordLabel.Name = "passwordLabel";
        this.passwordLabel.Text = "Password";
        // passwordTextBox
        this.passwordTextBox.Name = "passwordTextBox";
        this.passwordTextBox.Size = new System.Drawing.Size(190, 23);
        this.passwordTextBox.PasswordChar = '*';
        this.Padding = new Padding(0, 0, 0, 15);
        // loginButton
        this.loginButton.Name = "loginButton";
        this.loginButton.Text = "Login";
        this.loginButton.Size = new System.Drawing.Size(190, 30);
        // container
        this.loginPanel.Controls.Add(this.usernameLabel);
        this.loginPanel.Controls.Add(this.usernameTextBox);
        this.loginPanel.Controls.Add(this.passwordLabel);
        this.loginPanel.Controls.Add(this.passwordTextBox);
        this.loginPanel.Controls.Add(this.loginButton);
        this.loginPanel.Name = "loginPanel";
        this.loginPanel.Dock = DockStyle.Fill;
        this.loginPanel.FlowDirection = FlowDirection.TopDown;
        this.loginPanel.WrapContents = false;
        this.loginPanel.MinimumSize = new System.Drawing.Size(200, 150);
        this.loginPanel.Visible = true; // Initially visible

        // dashboardLabel
        this.dashboardLabel.AutoSize = true;
        this.dashboardLabel.Name = "dashboardLabel";
        this.dashboardLabel.Text = "Dashboard";
        // logoutButton
        this.logoutButton.Name = "logoutButton";
        this.logoutButton.Text = "Logout";
        this.logoutButton.Size = new System.Drawing.Size(190, 30);
        // raiseHandButton
        this.raiseHandButton.Name = "raiseHandButton";
        this.raiseHandButton.Text = "Raise Hand";
        this.raiseHandButton.Size = new System.Drawing.Size(190, 30);
        // container
        this.dashboardPanel.Controls.Add(this.dashboardLabel);
        this.dashboardPanel.Controls.Add(this.logoutButton);
        this.dashboardPanel.Controls.Add(this.raiseHandButton);
        this.dashboardPanel.Name = "dashboardPanel";
        this.dashboardPanel.Dock = DockStyle.Fill;
        this.dashboardPanel.FlowDirection = FlowDirection.TopDown;
        this.dashboardPanel.WrapContents = false;
        this.dashboardPanel.MinimumSize = new System.Drawing.Size(200, 150);
        this.dashboardPanel.Visible = false; // Initially hidden

        this.Controls.Add(this.loginPanel);
        this.Controls.Add(this.dashboardPanel);

        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(200, 150);
        this.MaximizeBox = false;
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.Text = "Form1";

        this.ResumeLayout(false);
    }

    #endregion

    private Label usernameLabel;
    private TextBox usernameTextBox;
    private Label passwordLabel;
    private TextBox passwordTextBox;
    private Button loginButton;
    // Layout container
    private FlowLayoutPanel loginPanel;

    private Label dashboardLabel;
    private Button logoutButton;
    private Button raiseHandButton;
    private FlowLayoutPanel dashboardPanel;
}
