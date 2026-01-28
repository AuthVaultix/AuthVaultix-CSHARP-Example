---

# AuthVaultix C# Example

This repository provides a **C# example implementation** for using the **AuthVaultix API ( v1.3)**.

AuthVaultix is a modern authentication & licensing system designed for **secure desktop loaders and protected applications**.

---

## ✨ Features

* API  v1.3 support
* Secure app initialization
* Username / password login
* License-based authentication
* Session validation & heartbeat checks
* ED25519 response signature verification
* Timestamp protection (anti-replay)
* Secure HWID-based device binding

---

## 📦 Included Examples

* Console example
* WinForms example
* API  v1.3 request flow
* Secure response validation
* Error handling & session safety

---

## 🖥️ HWID Generation

AuthVaultix uses a **hardware-bound HWID** to prevent account sharing and unauthorized access.

```csharp
"{cpu}|{board}|{disk}|{guid}|{sid}"
```

**HWID Components:**

* `cpu` – Processor ID
* `board` – Motherboard serial
* `disk` – Primary disk serial
* `guid` – Machine GUID
* `sid` – Windows User SID

These values are combined to generate a **unique and stable device identifier**.

---

## 🔐 Security Highlights

* ED25519 signed API responses
* Timestamp validation (anti-replay protection)
* Session hijack detection
* Redis-backed session storage (server-side)
* Invalid or tampered response auto-termination

---

## 🚀 Getting Started

```bash
git clone https://github.com/AuthVaultix/AuthVaultix-CSHARP-Example.git
```

1. Open the project in **Visual Studio**
2. Set your **App Name**, **Owner ID**, and **API Version**
3. Build and run the example

---

## 📄 License

This project is licensed under the **MIT License**.

---
