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
