# Deploying to Fly.io 🎈

Fly.io is an excellent choice for .NET apps. It allows you to run your Docker container close to your users.

## Prerequisites
1.  **Fly CLI**: [Install flyctl](https://fly.io/docs/hands-on/install-flyctl/)
    *   Windows (PowerShell): `iwr https://fly.io/install.ps1 -useb | iex`
2.  **Login**: Run `fly auth login` in your terminal.

## Deployment Steps

### 1. Initialize App
Run this in the `FleetManage.Api` folder:
```bash
fly launch
```
*   **App Name**: Give it a unique name (e.g., `fleet-manage-api`).
*   **Region**: Choose one close to you (e.g., `dfw` for Dallas, `iad` for Virginia).
*   **Database**: Yes! Select **Postgres**.
    *   Choose "Development" configuration (free/low cost) to start.
    *   Fly will automatically set the `DATABASE_URL` secret for you.
*   **Redis**: No (unless you need it later).
*   **Deploy now?**: Choose **No** (we need to set secrets first).

### 2. Set Secrets
Copy your Stripe keys and JWT key into Fly's secure storage:

```bash
fly secrets set Stripe__PublishableKey="pk_test_..."
fly secrets set Stripe__SecretKey="sk_test_..."
fly secrets set Jwt__Key="YOUR_SUPER_SECRET_KEY_MUST_BE_32_CHARS_LONG"
```
*(Note: We use double underscores `__` for nested config in .NET Environment Variables)*

### 4. Deploy
Now push your code:
```bash
fly deploy
```
Fly will build your `Dockerfile`, push it, and start the machine.
**Note:** The app is configured to automatically apply database migrations on startup, so your tables will be created automatically!

## Verify
Your API will be live at:
`https://fleet-manage-api.fly.dev/swagger`
