# 🎓 EduTrackCRM - Online Academy CRM System

[![ASP.NET Core 10](https://img.shields.io/badge/ASP.NET%20Core-10-purple)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)](https://www.microsoft.com/sql-server)
[![Entity Framework](https://img.shields.io/badge/EF%20Core-9-green)](https://docs.microsoft.com/ef/)
[![License](https://img.shields.io/badge/License-Proprietary-blue)](LICENSE)

A full-featured CRM system designed for an online education academy. The system manages the complete lifecycle from lead capture (via ads, social media, and web forms) through customer service, trial sessions, student enrollment, and ongoing student retention.
---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Tech Stack](#️-tech-stack)
- [Lead Pipeline (23 Stages)](#-lead-pipeline-23-stages)
- [Student Statuses (11 States)](#-student-statuses-11-states)
- [System Architecture](#-system-architecture)
- [Database Schema](#-database-schema)
- [Integration Services](#-integration-services)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Roles & Permissions](#-roles--permissions)
- [Reports Module](#-reports-module)
- [WhatsApp Integration](#-whatsapp-integration)
- [Deployment](#-deployment)
- [Environment Variables](#-environment-variables)
- [Development Tasks (39 Phases)](#-development-tasks-39-phases)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

---

## 🚀 Overview

**EduTrackCRM** is a comprehensive, full-featured CRM system designed specifically for online education academies. The system manages the complete student lifecycle from lead capture (via ads, social media, and web forms) through customer service, trial sessions, enrollment, and ongoing student retention.

### 🎯 Key Goals
- ✅ Centralize all lead sources into one unified profile
- ✅ Automate lead assignment and follow-up reminders
- ✅ Track every interaction from first contact to subscription
- ✅ Monitor active, at-risk, and inactive students
- ✅ Provide actionable reporting for managers and admins

---

## ✨ Key Features

### 📊 Lead Management
- **23-stage pipeline** from New Lead → Active Student
- Deduplication by phone/email on every intake
- Auto-assignment (round-robin) to CS agents
- Full activity timeline with immutable audit log
- Unlimited notes, tags, and follow-up scheduling

### 🎓 Student Lifecycle
- **11 student statuses** with full history tracking
- Lead to student conversion after subscription
- Inactive student tracker (7/30/90 day views)
- Re-activation workflow with contact attempt logging
- Session attendance tracking

### 📱 Multi-Source Lead Capture
- Facebook Lead Ads (Meta Graph API)
- Instagram Lead Ads (same pipeline)
- TikTok Lead Generation API
- WhatsApp Business API
- Messenger Platform API
- Website web forms (REST endpoint)
- Manual entry + Excel import

### 🤖 Automation & Reminders
- Hangfire background jobs for follow-ups
- Trial session reminders (1 hour before)
- Auto-advance lead stage on attendance
- Daily follow-up queue per agent
- Overdue follow-up highlighting

### 📈 Advanced Reporting
- **13+ reports** with Excel export
- Conversion funnel chart
- Agent & teacher performance metrics
- Lead source analytics
- Lost/stop reason breakdowns
- Inactive student recovery reports

### 💬 WhatsApp Integration
- Direct `wa.me` links from any profile
- Message templates with variable substitution
- Sent messages log (future: full API send)
- Template categories: Welcome, Follow-Up, Trial Reminder, Re-Engagement

### 🔐 Security & Audit
- Role-based access (Admin, Manager, TeamLeader, CS, Teacher)
- Complete activity log (who did what, when, from which IP)
- JWT authentication for API endpoints
- Rate limiting on public endpoints
- Automatic daily database backups

---

## 🛠️ Tech Stack

### Backend
| Component | Technology |
|-----------|------------|
| **Framework** | ASP.NET Core 10 MVC |
| **ORM** | Entity Framework Core 9+ |
| **Database** | SQL Server 2022 / Azure SQL |
| **Auth** | ASP.NET Core Identity + JWT |
| **Real-time** | SignalR |
| **Background Jobs** | Hangfire |
| **Caching** | Redis (StackExchange.Redis) |
| **File Storage** | Azure Blob Storage / MinIO |
| **Email** | SendGrid / Mailgun |
| **Logging** | Serilog + Seq |
| **API Docs** | Swagger / Scalar (NSwag) |
| **Mapping** | AutoMapper |
| **Validation** | FluentValidation |
| **Export** | EPPlus 7 |

### Frontend (MVC Views)
- **Bootstrap 5** - Responsive UI
- **Alpine.js** - Lightweight interactivity
- **Chart.js** - Dashboard charts
- **DataTables.js** - Sortable/filterable tables
- **Flatpickr** - Date/time pickers
- **Select2** - Searchable dropdowns
- **Toastr.js** - Toast notifications
- **SweetAlert2** - Confirmation dialogs

### DevOps & Infrastructure
- Docker + Docker Compose
- GitHub Actions (CI/CD)
- Azure App Service / VPS
- Let's Encrypt (Caddy) for SSL
- Uptime monitoring

---

## 📊 Lead Pipeline (23 Stages)

| # | Stage | Description |
|---|-------|-------------|
| 1 | New Lead | Initial entry |
| 2 | Assigned | Assigned to CS agent |
| 3 | First Contact | Agent made first contact |
| 4 | Contacted | Successfully reached |
| 5 | No Answer | Call unanswered |
| 6 | Wrong Number | Invalid contact |
| 7 | Busy | Customer busy |
| 8 | Requested Callback | Customer wants callback |
| 9 | Interested | Showed interest |
| 10 | Requested Details | Asked for more info |
| 11 | WhatsApp Sent | Message sent via WhatsApp |
| 12 | Trial Offered | Free session offered |
| 13 | Trial Scheduled | Session booked |
| 14 | Trial Reminder Sent | Reminder notification sent |
| 15 | Trial Attended | Session completed |
| 16 | Trial Missed | No-show |
| 17 | Follow Up Required | Needs follow-up |
| 18 | Parent Discussing | Guardian discussing |
| 19 | Interested But Delayed | Interest but timing issue |
| 20 | Not Interested | Declined |
| 21 | Lost Lead | Closed as lost |
| 22 | Subscribed | Paid subscription |
| 23 | Active Student | Currently enrolled |

---

## 📊 Student Statuses (11 States)

| Status | Description |
|--------|-------------|
| New Student | Just enrolled |
| Active Student | Regularly attending |
| Trial Student | In trial period |
| On Hold | Temporary pause |
| Vacation | Student on break |
| Renewal Follow Up | Subscription ending soon |
| At Risk | Missing sessions |
| Inactive Student | No recent activity |
| Dropped Student | Formally withdrawn |
| Transferred Teacher | Changed teacher |
| Graduated / Completed Course | Course finished |

---

## 🏗️ System Architecture

**Design Patterns:**
- Repository Pattern
- Service Layer Pattern
- Dependency Injection
- CQRS (for reporting)

---

## 🗄️ Database Schema Overview

### Core Tables
| Table | Purpose |
|-------|---------|
| `Leads` | All lead data, source metadata, campaign info |
| `Customers` | Promoted from leads after contact |
| `Students` | Promoted from customers after subscription |
| `Users` | CRM staff (CS agents, managers, admins, teachers) |
| `Roles` / `UserRoles` | ASP.NET Identity based |

### Lead & Activity
- `LeadStages` - Stage definitions
- `LeadAssignments` - Assignment history
- `LeadActivities` - Full timeline
- `LeadNotes` - Unlimited notes
- `LeadTags` - Many-to-many tags
- `LostReasons` - Lookup table

### Scheduling
- `TrialSessions` - Booking, teacher, date/time, attendance
- `FollowUps` - Scheduled follow-up tasks with reminders

### Students
- `StudentProfiles` - Full student info post-enrollment
- `StudentStatuses` - Current status + history
- `StudentStopReasons` - Why a student paused
- `ContactAttempts` - Re-engagement tracking

### Messaging
- `MessageTemplates` - WhatsApp/Messenger templates
- `SentMessages` - Log of all outbound messages

### System
- `ActivityLog` - Complete audit trail
- `Notifications` - Internal notification inbox
- `Tasks` - Staff task assignments
- `Documents` - Uploaded files
- `Campaigns` - Ad campaign reference data

---

## 🔌 Integration Services

### Lead Source Integrations
| Source | Service | Status |
|--------|---------|--------|
| Facebook Leads | Meta Marketing API (Graph API v19+) | ✅ Webhook ready |
| Instagram Leads | Same Meta Graph API | ✅ Webhook ready |
| Website Forms | Internal REST endpoint | ✅ Built |
| WhatsApp | WhatsApp Business API | ⚠️ Template ready |
| Messenger | Meta Messenger Platform API | ✅ Webhook ready |
| TikTok Leads | TikTok Lead Generation API | ✅ Webhook ready |

---



