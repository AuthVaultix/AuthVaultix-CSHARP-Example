# AuthVaultix-CSHARP-Example ⭐ Please star 🌟

AuthVaultix C# example SDK for the **AuthVaultix** licensing & authentication API.

---

## 🐛 Bugs

If you are using our example with **no significant changes** and you are having problems, please open an issue on GitHub.

However, we do **NOT** provide support for integrating AuthVaultix into your own custom project. If you can't figure this out, use Google or YouTube to learn more about the C# programming language.

---

## 🔐 Security Practices

- Utilize **obfuscation** provided by companies such as DotNet Reactor or Themida (utilize their SDKs too for greater protection)
- Perform **frequent integrity checks** to ensure the memory of the program has not been modified
- Don't write the bytes of a file you've downloaded to disk if you don't want the file to be retrieved by the user. Rather, **execute the file in memory** and erase it from memory the moment execution finishes

> While our API ensures license validation, it's crucial to implement robust client-side protection like obfuscation and integrity checks to prevent software tampering, as vulnerabilities often stem from insufficient client security.

---

## `AuthVaultixClient` Instance Definition

Visit your **AuthVaultix Dashboard**, select your application, and grab your credentials.

Replace the values in your `LoginForm.cs` (or `Program.cs`) file:

```cs
public static AuthVaultixClient Client = new AuthVaultixClient(
    appName: "", // App name
    ownerId: "", // Account ID
    secret: "", // App Secret
    version: "1.0" // App Version
);
```

---

## Initialize Application

You **must** call `Init()` before using any other AuthVaultix function. Otherwise no other function will work.

```cs
if (!Client.Init())
{
    MessageBox.Show(Client.RisponceCollection);
    Environment.Exit(0);
}
```

---

## Check Session Validation

Use this to verify whether the user's current session is still valid.

```cs
if (Client.Check())
{
    MessageBox.Show(Client.RisponceCollection); // e.g. "Session valid"
}
```

---

## Check Blacklist Status

Check if the current HWID or IP Address is blacklisted. You can call this right after `Init()` so a blacklisted user can't even reach the login screen.

> If a blacklisted user tries to login/register, the server will deny them anyway — so this function is **optional but recommended** for speed.

```cs
string msg;
if (!Client.CheckBlacklist(out msg))
{
    MessageBox.Show(msg); // "You are blacklisted"
    Environment.Exit(0);
}
```

---

## Login with Username / Password

```cs
if (!Client.Login(userFild.Text, pasFild.Text))
{
    MessageBox.Show(Client.RisponceCollection, "Login Failed");
    return;
}
// Login successful — proceed to main form
MainForm main = new MainForm();
main.Show();
this.Hide();
```

---

## Register with Username / Password / License Key

```cs
if (!Client.Register(userFild.Text, pasFild.Text, keyFild.Text, emailFild.Text))
{
    MessageBox.Show(Client.RisponceCollection, "Registration Failed");
    return;
}
// Registration successful — proceed to main form
MainForm main = new MainForm();
main.Show();
this.Hide();
```

---
## Upgrade User (Username + New License Key)

Used so the user can add extra time or upgrade their subscription by claiming a new key.

> [!WARNING]
> No password is needed to upgrade an account. Unlike login and register — you should **NOT** log the user in after a successful upgrade. Just show a success message.

```cs
if (!Client.Upgrade(userFild.Text, keyFild.Text))
{
    MessageBox.Show(Client.RisponceCollection, "Upgrade Failed");
    return;
}
MessageBox.Show("Upgrade successful!", "Success");
// Do NOT proceed to MainForm here
```

---

## Login with Just a License Key

Users can use this if their license key has never been used before, OR if it has been used before (HWID match). If you plan to allow key-only access, you can remove the login and register buttons.

```cs
if (!Client.LicenseLogin(keyFild.Text))
{
    MessageBox.Show(Client.RisponceCollection, "License Login Failed");
    return;
}
// License login successful
MainForm main = new MainForm();
main.Show();
this.Hide();
```

---

## Forgot Password

Allow users to reset their password via email.

```cs
if (!Client.ForgotPassword(userFild.Text, keyFild.Text))
{
    MessageBox.Show(Client.RisponceCollection, "Forgot Password Failed");
    return;
}
MessageBox.Show("Reset email sent successfully", "Success");
// Do NOT proceed to MainForm — user hasn't authenticated yet
```

---

## User Data

Display information for the currently logged-in user.

```cs
Console.WriteLine("Username: "      + Client.CurrentUser.username);
Console.WriteLine("IP Address: "    + Client.CurrentUser.ip);
Console.WriteLine("HWID: "          + Client.CurrentUser.hwid);
Console.WriteLine("Created At: "    + Client.CurrentUser.CreationDateFormatted);
Console.WriteLine("Last Login: "    + Client.CurrentUser.LastLoginFormatted);
Console.WriteLine("License Key: "   + Client.CurrentUser.subscriptions[0].key);
Console.WriteLine("Subscription: "  + Client.CurrentUser.subscriptions[0].subscription);
Console.WriteLine("Expires: "       + Client.CurrentUser.subscriptions[0].ExpiryFormatted);
Console.WriteLine("Time Left: "     + Client.CurrentUser.subscriptions[0].TimeLeft);
```

---

## Check Subscription Name of User

If you want to wall off parts of your app to specific subscription tiers, use the subscription name. When you create licenses on the dashboard, each license maps to a subscription level/name.

```cs
var sub = Client.CurrentUser.subscriptions[0].subscription;

if (sub == "default")
{
    Console.WriteLine("User has Default subscription.");
}
else if (sub == "premium")
{
    Console.WriteLine("User has Premium subscription.");
}
```

---

## Show List of Online Users

```cs
List<OnlineUser> onlineUsers;
string msg;

if (Client.FetchOnline(out onlineUsers, out msg))
{
    foreach (var user in onlineUsers)
    {
        Console.WriteLine("Online: " + user.credential);
    }
}
else
{
    Console.WriteLine("Error: " + msg);
}
```

---

## Application Variables (Global Variables)

A string stored server-side on the AuthVaultix dashboard. Can be set as **authenticated** (only logged-in users can access) or **public** (accessible before login). These are global and static for all users.

```cs
string val = Client.GetGlobalVar("variableNameHere");

if (val == null)
{
    MessageBox.Show(Client.RisponceCollection);
    return;
}
MessageBox.Show("Global variable value: " + val);
```

---

## User Variables

User variables are strings stored server-side and are **specific to each user**. You can set them from the dashboard, via SellerAPI, or directly from your loader.

**Set a user variable:**

```cs
if (!Client.SetVar("discord", "MyDiscord#0001"))
{
    MessageBox.Show(Client.RisponceCollection);
    return;
}
MessageBox.Show("User variable set successfully!");
```

**Get a user variable:**

```cs
string val = Client.GetVar("discord");

if (val == null)
{
    MessageBox.Show(Client.RisponceCollection);
    return;
}
MessageBox.Show("User variable value: " + val);
```

---

## Application Logs

Used to log data to your AuthVaultix dashboard. Great for anti-debug alerts, error tracking, or audit logs. Can be used **before or after login**.

```cs
string logMsg;
if (!Client.Log("User clicked the start button", out logMsg))
{
    MessageBox.Show(logMsg);
}
else
{
    MessageBox.Show(logMsg); // success message
}
```

---

## Ban the User

Ban the current user and blacklist their HWID and IP Address. Ideal to call when you detect a tampering/intrusion attempt.

**Function only works after login.**

```cs
string msg;
if (Client.Ban("Cheating detected", out msg))
{
    MessageBox.Show(msg, "Banned");
    MessageBox.Show("Please reopen this program.");
    Environment.Exit(0);
}
else
{
    MessageBox.Show(msg, "Ban Failed");
}
```

---

## Download File

Keep files secure by hosting them on the AuthVaultix dashboard. The download function returns raw bytes — you can save them to disk or execute them in memory.

`823785F2` is the File ID from your dashboard after uploading a file.

```cs
byte[] fileBytes;
string msg;

if (!Client.Download("823785F2", out fileBytes, out msg))
{
    MessageBox.Show(msg, "Download Failed");
    return;
}

if (fileBytes == null || fileBytes.Length == 0)
{
    MessageBox.Show("File data is empty.", "Error");
    return;
}

// Save to disk
string savePath = Path.Combine(@"C:\Path\To\Save\", "output.exe");
File.WriteAllBytes(savePath, fileBytes);
MessageBox.Show("Downloaded " + fileBytes.Length + " bytes", "Success");
```

---

## Chat Channels

Allow users to communicate amongst themselves inside your application.

**Fetch chat messages (runs on a Timer every 15 seconds):**

```cs
var messages = await Client.ChatFetch("channel_name");

chatroomGrid.Rows.Clear();

if (messages == null || messages.Count == 0)
{
    chatroomGrid.Rows.Insert(0, "AuthVaultix", "No chat messages", DateTime.Now);
    return;
}

foreach (var msg in messages)
{
    chatroomGrid.Rows.Insert(0,
        msg.author,
        msg.message,
        DateTimeOffset.FromUnixTimeSeconds(msg.timestamp).ToLocalTime().DateTime
    );
}
```

**Send a chat message:**

```cs
string responseMsg;
if (Client.ChatSend(chatMsgField.Text, "channel_name", out responseMsg))
{
    chatroomGrid.Rows.Insert(0, Client.CurrentUser.username, chatMsgField.Text, DateTime.Now);
    chatMsgField.Clear();
}
else
{
    MessageBox.Show(Client.LastResponseMessage);
}
```

---

## Logout

Log out the current user and invalidate the session.

```cs
Client.Logout();
// Session is now cleared — redirect to LoginForm
```

---

## Change Username

Allow the user to change their username. The session will be invalidated after a successful change, requiring the user to log in again.

```cs
try
{
    Client.ChangeUsername("newUsernameHere");
    MessageBox.Show("Username changed! Please log in again.");
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message, "Error");
}
```

---

## Copyright License

AuthVaultix is proprietary software. Unauthorized redistribution, reselling, or modification of the core library is strictly prohibited.

- You may **not** provide the software to third parties as a hosted or managed service.
- You may **not** remove or obscure any licensing, copyright, or other notices.
- You may **not** alter, bypass, or circumvent the license key functionality.

Thank you for your compliance — we work hard on the development of AuthVaultix and do not appreciate our copyright being infringed.

