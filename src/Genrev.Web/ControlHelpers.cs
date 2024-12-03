using DevExpress.Web.Mvc;
using DevExpress.Web;
using System.Reflection;
using System;

public class ComboBox
{
    public static void PreRenderByField(object sender, object modelObject, string lookupPropertyName)
    {
        if (modelObject != null)
        {
            object obj = ((sender is MVCxComboBox) ? sender : null);
            PropertyInfo property = modelObject.GetType().GetProperty(lookupPropertyName);
            if (property == null)
            {
                throw new ArgumentException("lookupPropertyName not found on modelObject");
            }

            ((ASPxComboBox)obj).SelectedItem = ((ASPxAutoCompleteBoxBase)obj).Items.FindByValue(property.GetValue(modelObject));
        }
    }

    public static void PreRender(object sender, object modelObject)
    {
        PreRenderByField(sender, modelObject, "ID");
    }
}