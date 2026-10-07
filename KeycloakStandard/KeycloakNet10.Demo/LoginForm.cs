namespace KeycloakNet10.Demo;

public sealed class LoginForm : Form
{
    private readonly TextBox _baseUrl = new() { Text = "http://localhost:8080/" };
    private readonly TextBox _clientId = new();
    private readonly TextBox _clientSecret = new() { UseSystemPasswordChar = true };
    private readonly TextBox _adminUsername = new();
    private readonly TextBox _adminPassword = new() { UseSystemPasswordChar = true };
    private readonly TextBox _username = new();
    private readonly TextBox _password = new() { UseSystemPasswordChar = true };

    public string BaseUrl => _baseUrl.Text.Trim();
    public string ClientId => _clientId.Text.Trim();
    public string ClientSecret => _clientSecret.Text;
    public string AdminUsername => _adminUsername.Text.Trim();
    public string AdminPassword => _adminPassword.Text;
    public string Username => _username.Text.Trim();
    public string Password => _password.Text;

    public LoginForm()
    {
        Text = "Keycloak Demo";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.None;
        Font = new Font(Font.FontFamily, Font.Size * 2);
        ClientSize = new Size(1060, 700);

        var rows = new (string Label, TextBox Box)[]
        {
            ("Base URL", _baseUrl),
            ("Client ID", _clientId),
            ("Client Secret", _clientSecret),
            ("Admin Username", _adminUsername),
            ("Admin Password", _adminPassword),
            ("User Username", _username),
            ("User Password", _password)
        };

        const int labelLeft = 40;
        const int labelWidth = 300;
        const int boxLeft = 360;
        const int boxWidth = 640;

        var y = 40;
        foreach (var (label, box) in rows)
        {
        Controls.Add(new Label { Text = label, Left = labelLeft, Top = y + 8, Width = labelWidth, AutoEllipsis = false, Font = new Font(Font.FontFamily, 8f) });
            box.Left = boxLeft;
            box.Top = y;
            box.Width = boxWidth;
            Controls.Add(box);
            y += 76;
        }

        var ok = new Button { Text = "OK", Left = 680, Top = y + 30, Width = 150, Height = 60 };
        var cancel = new Button { Text = "Cancel", Left = 850, Top = y + 30, Width = 150, Height = 60, DialogResult = DialogResult.Cancel };
        ok.Click += (_, _) =>
        {
            if (rows.Any(r => string.IsNullOrWhiteSpace(r.Box.Text)))
            {
                MessageBox.Show(this, "All fields are required.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
        };
        Controls.Add(ok);
        Controls.Add(cancel);
        AcceptButton = ok;
        CancelButton = cancel;
    }
}
