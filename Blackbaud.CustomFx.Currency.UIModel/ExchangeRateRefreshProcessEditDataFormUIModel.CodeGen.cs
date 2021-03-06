﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by BBUIModelLibrary
//     Version:  1.0.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Blackbaud.CustomFx.Currency.UIModel
{

/// <summary>
/// Represents the UI model for the 'Exchange Rate Refresh Process Edit Data Form' data form
/// </summary>
[global::Blackbaud.AppFx.UIModeling.Core.DataFormUIModelMetadata(global::Blackbaud.AppFx.UIModeling.Core.DataFormMode.Edit, "42ba2276-1976-4a6d-9599-8aea48902dec", "0e1b44fa-cb8a-4997-9f89-6ef4d8b5f9c9", "Exchange Rate Refresh Process")]
public partial class @ExchangeRateRefreshProcessEditDataFormUIModel : global::Blackbaud.AppFx.UIModeling.Core.DataFormUIModel
{

#region "Enums"

    /// <summary>
    /// Enumerated values for use with the DATECODE property
    /// </summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("BBUIModelLibrary", "1.0.0.0")]
	public enum DATECODES : int
	{
        @Latest = 0,
        @Historical = 1
	}

#endregion

#region "Extensibility methods"

	partial void OnCreated();

#endregion

    private global::Blackbaud.AppFx.UIModeling.Core.StringField _name;
    private global::Blackbaud.AppFx.UIModeling.Core.StringField _description;
    private global::Blackbaud.AppFx.UIModeling.Core.StringField _fixerapiaccesskey;
    private global::Blackbaud.AppFx.UIModeling.Core.ValueListField<DATECODES> _datecode;
    private global::Blackbaud.AppFx.UIModeling.Core.DateField _historicaldate;

	[System.CodeDom.Compiler.GeneratedCodeAttribute("BBUIModelLibrary", "1.0.0.0")]
	public @ExchangeRateRefreshProcessEditDataFormUIModel() : base()
	{


        _name = new global::Blackbaud.AppFx.UIModeling.Core.StringField();
        _description = new global::Blackbaud.AppFx.UIModeling.Core.StringField();
        _fixerapiaccesskey = new global::Blackbaud.AppFx.UIModeling.Core.StringField();
        _datecode = new global::Blackbaud.AppFx.UIModeling.Core.ValueListField<DATECODES>();
        _historicaldate = new global::Blackbaud.AppFx.UIModeling.Core.DateField();

        this.Mode = global::Blackbaud.AppFx.UIModeling.Core.DataFormMode.Edit;
        this.DataFormTemplateId = new System.Guid("42ba2276-1976-4a6d-9599-8aea48902dec");
        this.DataFormInstanceId = new System.Guid("0e1b44fa-cb8a-4997-9f89-6ef4d8b5f9c9");
        this.RecordType = "Exchange Rate Refresh Process";
        this.FORMHEADER.Value = "Edit an exchange rate refresh process";
        this.UserInterfaceUrl = "browser/htmlforms/custom/currency/ExchangeRateRefreshProcessEditDataForm.html";

        //
        //_name
        //
        _name.Name = "NAME";
        _name.Caption = "Name";
        _name.Required = true;
        _name.MaxLength = 100;
        this.Fields.Add(_name);
        //
        //_description
        //
        _description.Name = "DESCRIPTION";
        _description.Caption = "Description";
        _description.MaxLength = 255;
        this.Fields.Add(_description);
        //
        //_fixerapiaccesskey
        //
        _fixerapiaccesskey.Name = "FIXERAPIACCESSKEY";
        _fixerapiaccesskey.Caption = "Fixer API access key";
        _fixerapiaccesskey.Required = true;
        _fixerapiaccesskey.MaxLength = 32;
        this.Fields.Add(_fixerapiaccesskey);
        //
        //_datecode
        //
        _datecode.Name = "DATECODE";
        _datecode.Caption = "Use date";
        _datecode.Required = true;
        _datecode.DataSource.Add(new global::Blackbaud.AppFx.UIModeling.Core.ValueListItem<DATECODES> {Value = DATECODES.@Latest, Translation = "Latest"});
        _datecode.DataSource.Add(new global::Blackbaud.AppFx.UIModeling.Core.ValueListItem<DATECODES> {Value = DATECODES.@Historical, Translation = "Historical"});
        _datecode.Value = DATECODES.@Latest;
        this.Fields.Add(_datecode);
        //
        //_historicaldate
        //
        _historicaldate.Name = "HISTORICALDATE";
        _historicaldate.Caption = "Date";
        this.Fields.Add(_historicaldate);

		OnCreated();

	}

    /// <summary>
    /// Name
    /// </summary>
    [System.ComponentModel.Description("Name")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("BBUIModelLibrary", "1.0.0.0")]
	public global::Blackbaud.AppFx.UIModeling.Core.StringField @NAME {
		get { return _name; }
	}

    /// <summary>
    /// Description
    /// </summary>
    [System.ComponentModel.Description("Description")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("BBUIModelLibrary", "1.0.0.0")]
	public global::Blackbaud.AppFx.UIModeling.Core.StringField @DESCRIPTION {
		get { return _description; }
	}

    /// <summary>
    /// Fixer API access key
    /// </summary>
    [System.ComponentModel.Description("Fixer API access key")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("BBUIModelLibrary", "1.0.0.0")]
	public global::Blackbaud.AppFx.UIModeling.Core.StringField @FIXERAPIACCESSKEY {
		get { return _fixerapiaccesskey; }
	}

    /// <summary>
    /// Use date
    /// </summary>
    [System.ComponentModel.Description("Use date")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("BBUIModelLibrary", "1.0.0.0")]
	public global::Blackbaud.AppFx.UIModeling.Core.ValueListField<DATECODES> @DATECODE {
		get { return _datecode; }
	}

    /// <summary>
    /// Date
    /// </summary>
    [System.ComponentModel.Description("Date")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("BBUIModelLibrary", "1.0.0.0")]
	public global::Blackbaud.AppFx.UIModeling.Core.DateField @HISTORICALDATE {
		get { return _historicaldate; }
	}

}


}
