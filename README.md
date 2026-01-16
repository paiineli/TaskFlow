# TaskFlow - Support Ticket System

A full-featured ticket management system built with ASP.NET Core MVC. Allows customers to open support requests and track their progress, while support teams can manage, assign, and resolve tickets efficiently.

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoft-sql-server&logoColor=white)
![Bootstrap 5](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)

## Why I built this

I wanted something straightforward for managing support tickets without the bloat of commercial solutions. Most ticket systems are either too complicated or too limited. This gives you full control over the code while keeping things simple and practical.

Plus, it's a solid project to practice ASP.NET Core MVC with a clean architecture.

## What it does

- **Ticket creation** - Users can open tickets describing their issues
- **Categories** - Organize by type (Technical Support, Hardware, Software, Network, etc.)
- **Priority levels** - Low, Normal, High, and Urgent
- **Status workflow** - Tracks tickets from "Waiting" to "Closed"
- **Assignment system** - Agents can claim tickets or transfer them to others
- **Comments** - Communication between customers and support agents (with private notes)
- **File attachments** - Upload screenshots, logs, documents (prepared, not fully implemented)
- **Full audit trail** - Every action is logged in the history
- **Satisfaction ratings** - Customers can rate the support they received (1-5 stars)

## Tech stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server Express (works with LocalDB too)
- **ORM**: Dapper (because it's fast and straightforward)
- **Frontend**: Bootstrap 5, jQuery, FontAwesome
- **Authentication**: Cookie-based with Claims

## Project structure

```
TaskFlow/
├── Data/
│   └── AcessaDados.cs              # Database connection handling
├── Model/
│   ├── ChamadoMOD.cs               # Ticket model
│   ├── ChamadoComentarioMOD.cs     # Comment model
│   ├── ChamadoAnexoMOD.cs          # Attachment model
│   ├── ChamadoHistoricoMOD.cs      # History/audit model
│   ├── ChamadoAvaliacaoMOD.cs      # Rating model
│   ├── CategoriaMOD.cs             # Category model
│   └── ...                         # Other models
├── Repository/
│   ├── ChamadoREP.cs               # Main ticket repository
│   ├── ChamadoComentarioREP.cs     # Comment repository
│   └── ...                         # Other repositories
├── Controllers/
│   ├── ChamadoController.cs        # Ticket management
│   ├── HomeController.cs           # Dashboard
│   └── LoginController.cs          # Authentication
├── Views/
│   ├── Chamado/                    # Ticket views
│   ├── Home/                       # Dashboard
│   └── Shared/                     # Layouts and partials
├── Helpers/
│   ├── ControllerExtensions.cs     # View helpers (badges, icons, formatting)
│   └── FuncaoCriptografia.cs       # Encryption for IDs in URLs
└── IOC/
    └── DependencyContainer.cs      # Dependency injection setup
```

## Database schema

The system uses 12 tables:

- **TB_FUNCIONARIO** - Users (Admin, Agent, Customer)
- **TB_EMPRESA** - Companies
- **TB_CATEGORIA** - Ticket categories
- **TB_STATUS** - Ticket statuses
- **TB_JUSTIFICATIVA** - Closure/cancellation reasons
- **TB_CHAMADO** - Main tickets table
- **TB_CHAMADO_COMENTARIO** - Comments
- **TB_CHAMADO_ANEXO** - File attachments
- **TB_CHAMADO_HISTORICO** - Audit log
- **TB_CHAMADO_AVALIACAO** - Ratings

Check the `TaskFlow_Database_Script.sql` file for the complete schema.

## Getting started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express or LocalDB
- Visual Studio 2022 or VS Code (optional)

### Installation

1. **Clone the repo**
```bash
git clone https://github.com/yourusername/taskflow.git
cd taskflow
```

2. **Create the database**

Open SQL Server Management Studio (or Azure Data Studio) and run the `TaskFlow_Database_Script.sql` script. This will create the `TaskFlowDB` database with all tables, indexes, and sample data.

3. **Update connection string**

Edit `appsettings.json` with your SQL Server details:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TaskFlowDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

If you're using LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TaskFlowDB;Trusted_Connection=True;"
```

4. **Install dependencies**
```bash
dotnet restore
```

5. **Build and run**
```bash
dotnet build
dotnet run
```

The application will be available at `https://localhost:5001` (or the port shown in your console).

## Test accounts

The database script creates these users:

| Role | Username | Password | Description |
|------|----------|----------|-------------|
| Admin | `admin` | `123456` | System administrator |
| Agent | `joao.silva` | `123456` | Support agent |
| Agent | `maria.santos` | `123456` | Support agent |
| Customer | `ana.costa` | `123456` | Regular customer |

**Note**: Change these passwords in production!

## How to use

### As a customer

1. Log in with a customer account
2. Click "New Ticket" on the dashboard
3. Fill in the title, select a category, set priority, and describe the issue
4. Submit and track the ticket status
5. Add comments to communicate with support
6. Rate the support after the ticket is closed

### As a support agent

1. Log in with an agent account
2. View all open tickets or filter by category/status
3. Click "Assign to Me" to take ownership of a ticket
4. Update the status as you work (In Progress → Resolved → Closed)
5. Add comments (can be internal notes visible only to agents)
6. Transfer tickets to other agents if needed
7. Close or cancel tickets with justification

## Key features explained

### Ticket workflow

```
New Ticket → Waiting → In Analysis → In Progress → Resolved → Closed
                                         ↓
                                    Cancelled
```

- **Waiting**: Ticket just opened, needs assignment
- **In Analysis**: Being evaluated by support team
- **In Progress**: Actively being worked on
- **Awaiting Customer**: Waiting for customer response
- **Resolved**: Fixed, waiting for customer confirmation
- **Closed**: Finished and confirmed
- **Cancelled**: Cancelled with reason

### Permission system

The system has three user types:

- **Admin (A)**: Full access, can manage everything
- **Agent (U)**: Can view all tickets, claim, transfer, and resolve
- **Customer (C)**: Can only see their own tickets

Views and actions are automatically adjusted based on user type.

### Auto-generated ticket numbers

Tickets get sequential numbers like `CHM-2025-00001`, `CHM-2025-00002`, etc. The year resets the counter automatically.

### Private notes

Agents can add internal comments (marked "Internal" with a lock icon) that customers can't see. Useful for internal coordination without confusing the customer.

## Customization

### Adding new categories

```sql
INSERT INTO TB_CATEGORIA (NmCategoria, DsCategoria, CdUsuarioCadastro, SnAtivo) 
VALUES ('Your Category', 'Description here', 1, 'S');
```

### Adding new statuses

```sql
INSERT INTO TB_STATUS (TxStatus, DsStatus, NrOrdem, SnAtivo) 
VALUES ('Your Status', 'Description', 8, 'S');
```

### Changing ticket number format

Edit the stored procedure `SP_GERAR_NUMERO_CHAMADO` in the database if you want a different format (like `TKT-2025-00001` instead of `CHM-2025-00001`).

## Things I might add later

- [ ] Email notifications
- [ ] File upload implementation (structure is ready)
- [ ] SLA (Service Level Agreement) tracking
- [ ] Dashboard with charts/statistics
- [ ] Advanced reporting
- [ ] Multi-language support
- [ ] Dark mode
- [ ] Knowledge base / FAQ section
- [ ] Bulk operations

## Notes

- **Passwords are plain text** in the demo. In production, use BCrypt or ASP.NET Core Identity for proper hashing.
- **File uploads** - The database structure is ready, but the actual upload logic isn't implemented yet.
- **Error handling** - Basic try-catch blocks are there, but you might want to add more detailed logging.

## Contributing

Feel free to fork this and make it your own. If you find bugs or have suggestions, open an issue or submit a pull request.

## License

This project is open source and available under the MIT License. Do whatever you want with it.

## Credits

Built with:
- ASP.NET Core 8.0
- Dapper
- Bootstrap 5
- FontAwesome
- X.PagedList

---

Made this for learning and because I needed a simple ticket system. Hope it's useful for you too.
