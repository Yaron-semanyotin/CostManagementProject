<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="CostWise.Dashboard" MasterPageFile="~/Site.Master" Title="לוח בקרה" %>

<asp:Content ID="DashboardMainContent" ContentPlaceHolderID="MainContent" runat="server">

    <section aria-labelledby="DashboardHeading">
        <div class="card shadow-sm">
            <div class="card-body">
                <h2 id="DashboardHeading" class="h4 card-title">סקירת המערכת</h2>

                <p class="card-text text-secondary mb-0">
                    מכאן תוכל לעבור לניהול יחידות מידה, רכיבים, מוצרים וחישובי עלויות.
                </p>
            </div>
        </div>
    </section>

</asp:Content>
