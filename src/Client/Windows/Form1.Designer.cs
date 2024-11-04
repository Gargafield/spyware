namespace Windows;

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
        this.container = new FlowLayoutPanel();

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
        this.container.Controls.Add(this.usernameLabel);
        this.container.Controls.Add(this.usernameTextBox);
        this.container.Controls.Add(this.passwordLabel);
        this.container.Controls.Add(this.passwordTextBox);
        this.container.Controls.Add(this.loginButton);
        this.container.Name = "container";
        this.container.Dock = DockStyle.Fill;
        this.container.FlowDirection = FlowDirection.TopDown;
        this.container.WrapContents = false;
        this.container.MinimumSize = new System.Drawing.Size(200, 150);

        this.Controls.Add(this.container);

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
    private FlowLayoutPanel container;
}
