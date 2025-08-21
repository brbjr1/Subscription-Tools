Imports System.Web.UI.WebControls

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.UI.Utilities

Imports Ventrian.SubscriptionTools.Entities

Namespace Ventrian.SubscriptionTools

    Partial Public Class EditReceipt
        Inherits ModuleBase

#Region " Private Members "

        Private _receiptID As Integer = Null.NullInteger

#End Region

#Region " Private Methods "

        Private Sub ReadQueryString()

            If Not (Request("ReceiptID") Is Nothing) Then
                _receiptID = Convert.ToInt32(Request("ReceiptID"))
            End If

        End Sub

        Private Sub BindCrumbs()

            Dim crumbs As New ArrayList

            Dim objCrumbMain As New CrumbInfo
            objCrumbMain.Caption = Localization.GetString("Main", LocalResourceFile)
            objCrumbMain.Url = NavigateURL()
            crumbs.Add(objCrumbMain)

            Dim objCrumbEditReceipts As New CrumbInfo
            objCrumbEditReceipts.Caption = Localization.GetString("EditReceipts", LocalResourceFile)
            objCrumbEditReceipts.Url = EditUrl("EditReceipts")
            crumbs.Add(objCrumbEditReceipts)

            Dim objCrumbEditReceipt As New CrumbInfo
            If (_receiptID = Null.NullInteger) Then
                objCrumbEditReceipt.Caption = Localization.GetString("AddReceipt", LocalResourceFile)
            Else
                objCrumbEditReceipt.Caption = Localization.GetString("EditReceipt", LocalResourceFile)
            End If
            objCrumbEditReceipt.Url = EditUrl("ReceiptID", _receiptID.ToString(), "EditReceipt")
            crumbs.Add(objCrumbEditReceipt)

            rptBreadCrumbs.DataSource = crumbs
            rptBreadCrumbs.DataBind()

        End Sub

        Private Sub BindUsers()

            Dim objUserController As New UserController
            Dim objUsers As ArrayList = objUserController.GetUsers(Me.PortalId)

            ' Sort users by DisplayName alphabetically
            objUsers.Sort(New UserDisplayNameComparer())

            ' Add a default "Select User" option
            Dim defaultUser As New UserInfo
            defaultUser.UserID = -1
            defaultUser.DisplayName = "-- Select User --"
            objUsers.Insert(0, defaultUser)

            ' Set the data text field to show FirstName + LastName + / + DisplayName
            drpUsers.DataTextField = "DisplayName"
            drpUsers.DataValueField = "UserID"
            drpUsers.DataSource = objUsers
            drpUsers.DataBind()

            ' Customize the display text for each user (except the default option)
            For i As Integer = 1 To drpUsers.Items.Count - 1
                Dim user As UserInfo = CType(objUsers(i), UserInfo)
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

        Private Sub BindPlans()

            Dim objPlanController As New PlanController

            Dim objPlans As ArrayList = objPlanController.List(Me.PortalId, Me.ModuleId)

            For Each objPlan As PlanInfo In objPlans

                If (objPlan.IsActive) Then
                    Dim symbol As String = "$"
                    Select Case Me.Currency
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

                    Dim _details As String = ""

                    If (objPlan.BillingFrequencyType = FrequencyType.OneTimeFee) Then
                        _details = Localization.GetString("PermanentAccess", LocalResourceFile)
                        If (_details.Length > 0) Then
                            _details = _details.Replace("[FEE]", symbol & objPlan.ServiceFee.ToString("##0.00"))
                        End If
                    Else
                        If (objPlan.BillingFrequencyType = FrequencyType.FixedEndDate) Then
                            _details = Localization.GetString("FixedEndDate", LocalResourceFile)
                            If (_details.Length > 0) Then
                                _details = _details.Replace("[FEE]", symbol & objPlan.ServiceFee.ToString("##0.00"))
                                _details = _details.Replace("[PERIOD]", objPlan.EndDate.ToString("MMM, dd yyyy"))
                            End If
                        Else
                            If (objPlan.BillingPeriod = 1) Then
                                _details = Localization.GetString("FrequencyAccessSingular", LocalResourceFile)
                            Else
                                _details = Localization.GetString("FrequencyAccessPlural", LocalResourceFile)
                            End If
                            If (_details.Length > 0) Then
                                _details = _details.Replace("[FEE]", symbol & objPlan.ServiceFee.ToString("##0.00"))
                                _details = _details.Replace("[PERIOD]", objPlan.BillingPeriod.ToString())
                                _details = _details.Replace("[FREQUENCY]", Localization.GetString(objPlan.BillingFrequencyType.ToString(), Me.LocalResourceFile))
                            End If
                        End If
                    End If

                    drpSelectPlan.Items.Add(New ListItem(objPlan.Name & " - " & _details, objPlan.PlanID.ToString()))
                End If
            Next

        End Sub

        Private Sub BindReceipt()

            If (_receiptID = Null.NullInteger) Then

                txtProcessor.Text = Localization.GetString("Manual", Me.LocalResourceFile)

                pnlAddReceipt.Visible = True
                pnlDetail.Visible = False
                cmdDelete.Visible = False

            Else

                pnlAddReceipt.Visible = False
                pnlDetail.Visible = True
                cmdUpdate.Visible = False
                cmdDelete.Visible = True
                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" & Localization.GetString("Confirmation", LocalResourceFile) & "');")

                Dim objReceiptController As New ReceiptController
                Dim objReceipt As ReceiptInfo = objReceiptController.Get(_receiptID)

                If Not (objReceipt Is Nothing) Then
                    lblUserName.Text = objReceipt.UserName
                    lblPlan.Text = objReceipt.Name
                    lblCreateDate.Text = objReceipt.DateCreated.ToShortDateString()
                    lblStartDate.Text = objReceipt.DateStart.ToShortDateString()
                    If (objReceipt.DateEnd <> Null.NullDate) Then
                        lblEndDate.Text = objReceipt.DateEnd.ToShortDateString()
                    End If
                    lblProcessor.Text = objReceipt.Processor
                    If (objReceipt.Processor = "PayPal") Then
                        lnkTransactionID.Text = objReceipt.ProcessorTxID
                        lnkTransactionID.Target = "_blank"
                        If (Me.UseLiveProcessor) Then
                            lnkTransactionID.NavigateUrl = "https://www.paypal.com/vst/id=" & objReceipt.ProcessorTxID
                        Else
                            lnkTransactionID.NavigateUrl = "https://www.sandbox.paypal.com/vst/id=" & objReceipt.ProcessorTxID
                        End If
                        lnkTransactionID.Visible = True
                    Else
                        lblTransactionID.Text = objReceipt.ProcessorTxID
                        lblTransactionID.Visible = True
                    End If
                    lblServiceFee.Text = objReceipt.ServiceFee.ToString("##0.00")
                Else
                    Response.Redirect(NavigateURL("EditReceipt"), True)
                End If

            End If

        End Sub

#End Region

#Region " Event Handlers "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                ReadQueryString()

                If (_receiptID = Null.NullInteger Or _receiptID < 0) AndAlso (Request("UserID") IsNot Nothing) Then
                    Dim userID As Integer = Convert.ToInt32(Request("UserID"))
                    If userID > 0 Then
                        drpUsers.SelectedValue = userID.ToString()
                    End If
                End If

                BindCrumbs()
                cmdStartDate.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStartDate)

                If (IsPostBack = False) Then

                    BindUsers()
                    BindPlans()
                    BindReceipt()

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            Try
                If (Page.IsValid) Then

                    ' Validate required fields
                    If drpSelectPlan.SelectedValue Is Nothing OrElse String.IsNullOrEmpty(drpSelectPlan.SelectedValue) Then
                        ' Show error - no plan selected
                        Return
                    End If

                    If drpUsers.SelectedValue Is Nothing OrElse String.IsNullOrEmpty(drpUsers.SelectedValue) Then
                        ' Show error - no user selected
                        Return
                    End If

                    Dim objReceipt As New ReceiptInfo

                    objReceipt.ModuleID = Me.ModuleId
                    objReceipt.PortalID = Me.PortalId
                    objReceipt.Status = "Complete"
                    objReceipt.Processor = If(txtProcessor.Text, "Manual")
                    objReceipt.ProcessorTxID = If(txtTransactionID.Text, "")

                    Dim objPlanController As New PlanController
                    Dim planID As Integer = Convert.ToInt32(drpSelectPlan.SelectedValue)
                    Dim objPlan As PlanInfo = objPlanController.Get(planID)

                    ' Validate plan exists
                    If objPlan Is Nothing Then
                        ' Show error - plan not found
                        Return
                    End If

                    objReceipt.BillingFrequency = objPlan.BillingFrequency
                    objReceipt.BillingPeriod = objPlan.BillingPeriod
                    objReceipt.ServiceFee = objPlan.ServiceFee
                    objReceipt.Name = objPlan.Name

                    objReceipt.DateCreated = DateTime.Now
                    
                    ' Safely handle start date
                    Try
                        If Not String.IsNullOrEmpty(txtStartDate.Text) Then
                            objReceipt.DateStart = Convert.ToDateTime(txtStartDate.Text)
                        Else
                            objReceipt.DateStart = DateTime.Now
                        End If
                    Catch ex As Exception
                        ' If date parsing fails, use current date
                        objReceipt.DateStart = DateTime.Now
                    End Try

                    Dim selectedUserID As Integer = Convert.ToInt32(drpUsers.SelectedValue)
                    If selectedUserID <= 0 Then
                        ' Show error message
                        Return
                    End If

                    objReceipt.UserID = selectedUserID
                    
                    ' Get the user info to set the UserName and other user-related fields
                    Dim objUserController As New UserController
                    Dim objUser As UserInfo = objUserController.GetUser(Me.PortalId, selectedUserID)
                    If objUser IsNot Nothing Then
                        ' Safely set user properties with null checks
                        objReceipt.UserName = If(objUser.Username, "")
                        objReceipt.FirstName = If(objUser.FirstName, "")
                        objReceipt.LastName = If(objUser.LastName, "")
                        objReceipt.Email = If(objUser.Email, "")
                        objReceipt.DisplayName = If(objUser.DisplayName, "")
                    End If

                    ' Safely handle role logic
                    Dim objRoleController As New RoleController
                    Dim objUserRole As UserRoleInfo = Nothing
                    Try
                        objUserRole = objRoleController.GetUserRole(objReceipt.PortalID, objReceipt.UserID, objPlan.RoleID)
                    Catch ex As Exception
                        ' Role lookup failed, continue without role
                        objUserRole = Nothing
                    End Try

                    If Not (objUserRole Is Nothing) Then
                        Try
                            If (objUserRole.ExpiryDate <> Null.NullDate) Then
                                If (objUserRole.ExpiryDate > DateTime.Now) Then
                                    objReceipt.DateStart = objUserRole.ExpiryDate
                                End If
                            End If
                        Catch ex As Exception
                            ' Date comparison failed, continue with current date
                        End Try
                    End If

                    ' Safely calculate end date
                    Try
                        Select Case objPlan.BillingFrequencyType
                            Case FrequencyType.OneTimeFee
                                objReceipt.DateEnd = Null.NullDate
                                Exit Select
                            Case FrequencyType.Day
                                objReceipt.DateEnd = objReceipt.DateStart.AddDays(objPlan.BillingPeriod)
                                Exit Select
                            Case FrequencyType.Week
                                objReceipt.DateEnd = objReceipt.DateStart.AddDays(objPlan.BillingPeriod * 7)
                                Exit Select
                            Case FrequencyType.Month
                                objReceipt.DateEnd = objReceipt.DateStart.AddMonths(objPlan.BillingPeriod)
                                Exit Select
                            Case FrequencyType.Year
                                objReceipt.DateEnd = objReceipt.DateStart.AddYears(objPlan.BillingPeriod)
                                Exit Select
                            Case FrequencyType.FixedEndDate
                                objReceipt.DateEnd = objPlan.EndDate
                                Exit Select
                        End Select
                    Catch ex As Exception
                        ' Date calculation failed, set to null
                        objReceipt.DateEnd = Null.NullDate
                    End Try

                    ' Add the receipt
                    Dim objReceiptController As New ReceiptController
                    Try
                        objReceiptController.Add(objReceipt)
                    Catch ex As Exception
                        ' Receipt creation failed
                        Return
                    End Try

                    ' Handle role addition - only if checkbox is checked
                    If (chkAddToRole.Checked) Then
                        Try
                            ' Add the user to the role (DNN will handle duplicates gracefully)
                            objRoleController.AddUserRole(objReceipt.PortalID, objReceipt.UserID, objPlan.RoleID, objReceipt.DateStart, objReceipt.DateEnd)
                        Catch ex As Exception
                            ' Role assignment failed, but receipt was created
                            ' Log the error for debugging
                            DotNetNuke.Services.Exceptions.LogException(ex)
                        End Try
                    End If

                    Response.Redirect(EditUrl("EditReceipts"), True)

                End If

            Catch ex As Exception
                ' Log the error and show user-friendly message
                ProcessModuleLoadException(Me, ex)
            End Try

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click

            Response.Redirect(EditUrl("EditReceipts"), True)

        End Sub

        Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click

            Dim objReceiptController As New ReceiptController
            objReceiptController.Delete(_receiptID)

            Response.Redirect(EditUrl("EditReceipts"), True)

        End Sub

        Private Sub ValidateUserSelection(ByVal source As System.Object, ByVal e As System.Web.UI.WebControls.ServerValidateEventArgs)

            Dim selectedUserID As Integer = Convert.ToInt32(drpUsers.SelectedValue)
            If selectedUserID <= 0 Then
                e.IsValid = False
            Else
                e.IsValid = True
            End If

        End Sub

#End Region

    End Class

    Public Class UserDisplayNameComparer
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim user1 As UserInfo = CType(x, UserInfo)
            Dim user2 As UserInfo = CType(y, UserInfo)

            If user1 Is Nothing AndAlso user2 Is Nothing Then
                Return 0
            ElseIf user1 Is Nothing Then
                Return -1
            ElseIf user2 Is Nothing Then
                Return 1
            Else
                ' Build the display text for comparison (same logic as in BindUsers)
                Dim displayText1 As String = BuildUserDisplayText(user1)
                Dim displayText2 As String = BuildUserDisplayText(user2)
                
                Return String.Compare(displayText1, displayText2, True)
            End If
        End Function

        Private Function BuildUserDisplayText(ByVal user As UserInfo) As String
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
            
            Return displayText
        End Function
    End Class

End Namespace