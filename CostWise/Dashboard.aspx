<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs"
    Inherits="CostWise.Dashboard" MasterPageFile="~/Site.Master" Title="דף בית" %>

<asp:Content ID="DashboardMainContent" ContentPlaceHolderID="MainContent" runat="server">
    <header class="mb-4">
        <h2 id="DashboardHeading" class="mb-1">דף הבית</h2>
        <p class="text-secondary mb-0">גישה מהירה לאזורי הניהול המרכזיים של Tamhiro.</p>
    </header>

    <section aria-labelledby="QuickActionsHeading">
        <h3 id="QuickActionsHeading" class="h5 mb-3">פעולות מהירות</h3>

        <div class="row g-3">
            <div class="col-12 col-sm-6 col-xl-4">
                <a href="<%: ResolveUrl("~/MeasurementUnits.aspx") %>" class="dashboard-nav-card">
                    <span class="dashboard-nav-icon mb-3"><i class="bi bi-rulers" aria-hidden="true"></i></span>
                    <strong class="d-block mb-1">יחידות מידה</strong>
                    <span class="text-secondary small">ניהול יחידות מערכת ויחידות מותאמות לעסק.</span>
                </a>
            </div>

            <div class="col-12 col-sm-6 col-xl-4">
                <a href="<%: ResolveUrl("~/Ingredients.aspx") %>" class="dashboard-nav-card">
                    <span class="dashboard-nav-icon mb-3"><i class="bi bi-basket" aria-hidden="true"></i></span>
                    <strong class="d-block mb-1">רכיבים</strong>
                    <span class="text-secondary small">ניהול מחירי אריזה, כמויות ויחידות מידה.</span>
                </a>
            </div>

            <div class="col-12 col-sm-6 col-xl-4">
                <a href="<%: ResolveUrl("~/Products.aspx") %>" class="dashboard-nav-card">
                    <span class="dashboard-nav-icon mb-3"><i class="bi bi-box-seam" aria-hidden="true"></i></span>
                    <strong class="d-block mb-1">מוצרים ומתכונים</strong>
                    <span class="text-secondary small">יצירת מוצרים, בניית מתכונים וחישוב עלויות.</span>
                </a>
            </div>

            <div class="col-12 col-sm-6 col-xl-4">
                <a href="<%: ResolveUrl("~/CalculationHistory.aspx") %>" class="dashboard-nav-card">
                    <span class="dashboard-nav-icon mb-3"><i class="bi bi-clock-history" aria-hidden="true"></i></span>
                    <strong class="d-block mb-1">היסטוריית חישובים</strong>
                    <span class="text-secondary small">צפייה בחישובים קודמים ובמגמות עלות.</span>
                </a>
            </div>

            <div class="col-12 col-sm-6 col-xl-4">
                <a href="<%: ResolveUrl("~/BusinessProfile.aspx") %>" class="dashboard-nav-card">
                    <span class="dashboard-nav-icon mb-3"><i class="bi bi-building" aria-hidden="true"></i></span>
                    <strong class="d-block mb-1">פרופיל העסק</strong>
                    <span class="text-secondary small">פרטי העסק, הגדרות תצוגה ולוגו.</span>
                </a>
            </div>
        </div>
    </section>
</asp:Content>