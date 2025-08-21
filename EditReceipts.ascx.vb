Imports System.Web.UI.WebControls

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Imports Ventrian.SubscriptionTools.Entities

Namespace Ventrian.SubscriptionTools

    Partial Public Class EditReceipts
        Inherits ModuleBase

#Region " Private Members "

        Dim _currentPage As Integer = 1
        Dim _pageRecords As Integer = Null.NullInteger

        Dim _receipts As ArrayList

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If Not (Request("CurrentPage") Is Nothing) Then
                _currentPage = Convert.ToInt32(Request("CurrentPage"))
            End If

            If Not (Request("PageRecords") Is Nothing) Then
                _pageRecords = Convert.ToInt32(Request("PageRecords"))
            End If

            If (_pageRecords <> Null.NullInteger) Then
                If Not (drpRecordsPerPage.Items.FindByValue(_pageRecords.ToString()) Is Nothing) Then
                    drpRecordsPerPage.SelectedValue = _pageRecords.ToString()
                End If
            End If

            If Not (Request("UserID") Is Nothing) Then
                Dim userID As Integer = Convert.ToInt32(Request("UserID"))
                If userID > 0 Then
                    drpUsers.SelectedValue = userID.ToString()
                End If
            End If

        End Sub

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim objCrumbMain As New CrumbInfo
            objCrumbMain.Caption = Localization.GetString("Main", LocalResourceFile)
            objCrumbMain.Url = NavigateURL()
            crumbs.Add(objCrumbMain)

            Dim objEditReceipts As New CrumbInfo
            objEditReceipts.Caption = Localization.GetString("EditReceipts", LocalResourceFile)
            objEditReceipts.Url = EditUrl("EditReceipts")
            crumbs.Add(objEditReceipts)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

        Private Sub BindUsers()

            Dim objUserController As New DotNetNuke.Entities.Users.UserController
            Dim objUsers As ArrayList = objUserController.GetUsers(Me.PortalId)

            ' Sort users by DisplayName alphabetically
            objUsers.Sort(New UserDisplayNameComparer())

            ' Add a default "All Users" option
            Dim defaultUser As New DotNetNuke.Entities.Users.UserInfo
            defaultUser.UserID = -1
            defaultUser.DisplayName = "-- All Users --"
            objUsers.Insert(0, defaultUser)

            ' Set the data text field to show FirstName + LastName + / + DisplayName
            drpUsers.DataTextField = "DisplayName"
            drpUsers.DataValueField = "UserID"
            drpUsers.DataSource = objUsers
            drpUsers.DataBind()

            ' Customize the display text for each user (except the default option)
            For i As Integer = 1 To drpUsers.Items.Count - 1
                Dim user As DotNetNuke.Entities.Users.UserInfo = CType(objUsers(i), DotNetNuke.Entities.Users.UserInfo)
                Dim displayText As String = ""
                
                ' Build the display text: FirstName + LastName + / + DisplayName
                If Not String.IsNullOrEmpty(user.FirstName) AndAlso Not String.IsNullOrEmpty(user.LastName) Then
                    displayText = user.FirstName.Trim() & " " & user.LastName.Trim()
                ElseIf Not String.IsNullOrEmpty(user.FirstName) Then
                    displayText = user.FirstName.Trim()
                ElseIf Not String.IsNullOrEmpty(user.LastName) Then
                    displayText = user.LastName.Trim()
                End If
                
                ' Add the DisplayName if it exists and is different
                If Not String.IsNullOrEmpty(user.DisplayName) AndAlso displayText <> user.DisplayName Then
                    If displayText.Length > 0 Then
                        displayText = displayText & " / " & user.DisplayName
                    Else
                        displayText = user.DisplayName
                    End If
                End If
                
                ' Set the display text
                drpUsers.Items(i).Text = displayText
            Next
        End Sub

        Private Sub BindReceipts()

            Dim PageSize As Integer = Convert.ToInt32(drpRecordsPerPage.SelectedItem.Value)

            Dim objReceiptController As New ReceiptController

            Localization.LocalizeDataGrid(grdReceipts, Me.LocalResourceFile)

            ' Get the selected user ID for filtering
            Dim selectedUserID As Integer = Convert.ToInt32(drpUsers.SelectedValue)
            Dim userNameFilter As String = ""
            
            ' If a specific user is selected (not "All Users"), get their username for filtering
            If selectedUserID > 0 Then
                Dim objUserController As New DotNetNuke.Entities.Users.UserController
                Dim objUser As DotNetNuke.Entities.Users.UserInfo = objUserController.GetUser(Me.PortalId, selectedUserID)
                If objUser IsNot Nothing Then
                    userNameFilter = objUser.Username
                End If
            End If
            
            _receipts = objReceiptController.List(Me.PortalId, Me.ModuleId, Null.NullInteger, userNameFilter)

            Dim objPagedDataSource As New PagedDataSource

            objPagedDataSource.AllowPaging = True
            objPagedDataSource.DataSource = _receipts
            If (_pageRecords = Null.NullInteger) Then
                objPagedDataSource.PageSize = PageSize
            Else
                objPagedDataSource.PageSize = _pageRecords
            End If
            objPagedDataSource.CurrentPageIndex = _currentPage - 1

            If (_receipts.Count > 0) Then

                grdReceipts.DataSource = objPagedDataSource
                grdReceipts.DataBind()

                grdReceipts.Visible = True
                lblNoReceipts.Visible = False
                ctlPagingControl.Visible = True

                ctlPagingControl.Visible = True
                ctlPagingControl.TotalRecords = _receipts.Count
                ctlPagingControl.PageSize = PageSize
                ctlPagingControl.CurrentPage = _currentPage

                ctlPagingControl.QuerystringParams = "ctl=EditReceipts&mid=" & Me.ModuleId.ToString()

                If (Request("UserID") <> "") Then
                    ctlPagingControl.QuerystringParams = ctlPagingControl.QuerystringParams & "&UserID=" + Request("UserID")
                End If

                If (drpRecordsPerPage.SelectedIndex <> 0) Then
                    ctlPagingControl.QuerystringParams = ctlPagingControl.QuerystringParams & "&PageRecords=" & drpRecordsPerPage.SelectedValue
                End If

                ctlPagingControl.TabID = TabId
            Else
                grdReceipts.Visible = False
                lblNoReceipts.Visible = True
                ctlPagingControl.Visible = False
            End If

        End Sub

#End Region

#Region " Protected Methods "

        Protected Function GetDateEnd(ByVal obj As Object) As String

            Dim objReceipt As ReceiptInfo = CType(obj, ReceiptInfo)

            If Not (objReceipt Is Nothing) Then
                If (objReceipt.DateEnd <> Null.NullDate) Then
                    Return objReceipt.DateEnd.ToShortDateString()
                End If
            End If

            Return ""

        End Function

        Protected Function GetServiceFee(ByVal obj As Object) As String

            Dim objReceipt As ReceiptInfo = CType(obj, ReceiptInfo)

            If Not (objReceipt Is Nothing) Then

                Dim currency As String = Me.Currency
                If (objReceipt.Currency <> "") Then
                    currency = objReceipt.Currency
                End If

                Dim symbol As String = "$"
                Select Case currency.ToUpper()
                    Case "USD"
                        symbol = "$"

                    Case "GBP"
                        symbol = "�"

                    Case "EUR"
                        symbol = "�"

                    Case "CAD"
                        symbol = "$"

                    Case "JPY"
                        symbol = "�"
                End Select

                Return symbol & objReceipt.ServiceFee.ToString("##0.00")
            End If

            Return ""

        End Function

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                If (Page.IsPostBack = False) Then
                    ReadQueryString()
                    BindCrumbs()
                    BindUsers()
                    BindReceipts()
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub drpRecordsPerPage_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles drpRecordsPerPage.SelectedIndexChanged

            Try

                Response.Redirect(EditUrl("UserID", drpUsers.SelectedValue, "EditReceipts", "PageRecords=" & drpRecordsPerPage.SelectedValue), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub drpUsers_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles drpUsers.SelectedIndexChanged

            Try

                Response.Redirect(EditUrl("UserID", drpUsers.SelectedValue, "EditReceipts"), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdAddReceipt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddReceipt.Click

            Try

                Response.Redirect(EditUrl("EditReceipt"), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdReturnToModule_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReturnToModule.Click

            Try

                Response.Redirect(NavigateURL(), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub



        Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

            Try

                Response.Redirect(EditUrl("EditReceipts"), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

    End Class



End Namespace