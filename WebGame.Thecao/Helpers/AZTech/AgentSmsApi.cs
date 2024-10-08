//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by Web Services Description Language Utility
//Mono Framework v2.0.50727.1433
//


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="AgentSmsApiSoapBinding", Namespace="http://internalws.beyu.com/")]
public partial class AgentSmsApi : System.Web.Services.Protocols.SoapHttpClientProtocol {
    
    private System.Threading.SendOrPostCallback getUsageReportOperationCompleted;
    
    private System.Threading.SendOrPostCallback getCreditOperationCompleted;
    
    private System.Threading.SendOrPostCallback sendSmsOperationCompleted;
    
    /// <remarks/>
    public AgentSmsApi() {
        this.Url = "http://brand.aztech.com.vn/ws/agentSmsApiSoap";
    }
    
    /// <remarks/>
    public event getUsageReportCompletedEventHandler getUsageReportCompleted;
    
    /// <remarks/>
    public event getCreditCompletedEventHandler getCreditCompleted;
    
    /// <remarks/>
    public event sendSmsCompletedEventHandler sendSmsCompleted;
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:getUsageReport", RequestNamespace="http://internalws.beyu.com/", ResponseNamespace="http://internalws.beyu.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public reportSmsApiModel getUsageReport([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string authenticateUser, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string authenticatePass, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string fromDate, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string toDate) {
        object[] results = this.Invoke("getUsageReport", new object[] {
                    authenticateUser,
                    authenticatePass,
                    fromDate,
                    toDate});
        return ((reportSmsApiModel)(results[0]));
    }
    
    /// <remarks/>
    public void getUsageReportAsync(string authenticateUser, string authenticatePass, string fromDate, string toDate) {
        this.getUsageReportAsync(authenticateUser, authenticatePass, fromDate, toDate, null);
    }
    
    /// <remarks/>
    public void getUsageReportAsync(string authenticateUser, string authenticatePass, string fromDate, string toDate, object userState) {
        if ((this.getUsageReportOperationCompleted == null)) {
            this.getUsageReportOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetUsageReportOperationCompleted);
        }
        this.InvokeAsync("getUsageReport", new object[] {
                    authenticateUser,
                    authenticatePass,
                    fromDate,
                    toDate}, this.getUsageReportOperationCompleted, userState);
    }
    
    private void OngetUsageReportOperationCompleted(object arg) {
        if ((this.getUsageReportCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getUsageReportCompleted(this, new getUsageReportCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:getCredit", RequestNamespace="http://internalws.beyu.com/", ResponseNamespace="http://internalws.beyu.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string getCredit([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string authenticateUser, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string authenticatePass) {
        object[] results = this.Invoke("getCredit", new object[] {
                    authenticateUser,
                    authenticatePass});
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public void getCreditAsync(string authenticateUser, string authenticatePass) {
        this.getCreditAsync(authenticateUser, authenticatePass, null);
    }
    
    /// <remarks/>
    public void getCreditAsync(string authenticateUser, string authenticatePass, object userState) {
        if ((this.getCreditOperationCompleted == null)) {
            this.getCreditOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetCreditOperationCompleted);
        }
        this.InvokeAsync("getCredit", new object[] {
                    authenticateUser,
                    authenticatePass}, this.getCreditOperationCompleted, userState);
    }
    
    private void OngetCreditOperationCompleted(object arg) {
        if ((this.getCreditCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getCreditCompleted(this, new getCreditCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:sendSms", RequestNamespace="http://internalws.beyu.com/", ResponseNamespace="http://internalws.beyu.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string sendSms([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string authenticateUser, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string authenticatePass, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string brandName, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string message, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string receiver, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int type) {
        object[] results = this.Invoke("sendSms", new object[] {
                    authenticateUser,
                    authenticatePass,
                    brandName,
                    message,
                    receiver,
                    type});
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public void sendSmsAsync(string authenticateUser, string authenticatePass, string brandName, string message, string receiver, int type) {
        this.sendSmsAsync(authenticateUser, authenticatePass, brandName, message, receiver, type, null);
    }
    
    /// <remarks/>
    public void sendSmsAsync(string authenticateUser, string authenticatePass, string brandName, string message, string receiver, int type, object userState) {
        if ((this.sendSmsOperationCompleted == null)) {
            this.sendSmsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendSmsOperationCompleted);
        }
        this.InvokeAsync("sendSms", new object[] {
                    authenticateUser,
                    authenticatePass,
                    brandName,
                    message,
                    receiver,
                    type}, this.sendSmsOperationCompleted, userState);
    }
    
    private void OnsendSmsOperationCompleted(object arg) {
        if ((this.sendSmsCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.sendSmsCompleted(this, new sendSmsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    public new void CancelAsync(object userState) {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://internalws.beyu.com/")]
public partial class reportSmsApiModel {
    
    private string totalMessageField;
    
    private string mobiSmscField;
    
    private string vinaSmscField;
    
    private string viettelSmscField;
    
    private string vietnammobileSmscField;
    
    private string beelineSmscField;
    
    private string totalPriceField;
    
    private string totalBrandField;
    
    private int statusField;
    
    private string fromDateField;
    
    private string toDateField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string totalMessage {
        get {
            return this.totalMessageField;
        }
        set {
            this.totalMessageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string mobiSmsc {
        get {
            return this.mobiSmscField;
        }
        set {
            this.mobiSmscField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string vinaSmsc {
        get {
            return this.vinaSmscField;
        }
        set {
            this.vinaSmscField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string viettelSmsc {
        get {
            return this.viettelSmscField;
        }
        set {
            this.viettelSmscField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string vietnammobileSmsc {
        get {
            return this.vietnammobileSmscField;
        }
        set {
            this.vietnammobileSmscField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string beelineSmsc {
        get {
            return this.beelineSmscField;
        }
        set {
            this.beelineSmscField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string totalPrice {
        get {
            return this.totalPriceField;
        }
        set {
            this.totalPriceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string totalBrand {
        get {
            return this.totalBrandField;
        }
        set {
            this.totalBrandField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int status {
        get {
            return this.statusField;
        }
        set {
            this.statusField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string fromDate {
        get {
            return this.fromDateField;
        }
        set {
            this.fromDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string toDate {
        get {
            return this.toDateField;
        }
        set {
            this.toDateField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
public delegate void getUsageReportCompletedEventHandler(object sender, getUsageReportCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class getUsageReportCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal getUsageReportCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public reportSmsApiModel Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((reportSmsApiModel)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
public delegate void getCreditCompletedEventHandler(object sender, getCreditCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class getCreditCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal getCreditCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
public delegate void sendSmsCompletedEventHandler(object sender, sendSmsCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "0.0.0.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class sendSmsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal sendSmsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}
