using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Text;
using System.Xml;

public enum DBHFieldType
{
    Integer,
    String,
    Boolean,
    Array,
    Dictionary,
    Float,
    Resource
}

[GlobalClass]
public partial class DBHField : GodotObject
{
    public string FieldName { get; set; } = string.Empty;
    public DBHFieldType FieldType { get; set; }
    public string ResourceType { get; set; }
    public string DefaultValue { get; set; }
    public bool IsNullable { get; set; } = true;
    public bool IsId { get; set; } = false;
    public string Hint { get; set; } = "";
    public bool GenerateInCSharp { get; set; } = true;

    public string ToCSharpField()
    {
        string type = FieldType switch
        {
            DBHFieldType.Integer => "int",
            DBHFieldType.String => "string",
            DBHFieldType.Boolean => "bool",
            DBHFieldType.Array => "Array",
            DBHFieldType.Dictionary => "Dictionary",
            DBHFieldType.Float => "float",
            DBHFieldType.Resource => ResourceType,
            _ => throw new Exception($"Not implemented field type to string convertion: {FieldType} for field: {FieldName}"),
        };
        var result = $"public {type} {FieldName} {{ get; set; }}";

        if (DefaultValue != null)
        {
            if (FieldType == DBHFieldType.String)
                result += $" = \"{DefaultValue}\";";
            else
                result += $" = {DefaultValue};";
            // TODO: Implement more special cases like Dictionary and Array
        }

        return result;
    }
}

[GlobalClass]
public partial class DBHEntry : GodotObject
{
    public Dictionary<DBHField, Variant> Values = new();
    public int Id;
    public string Class;

    public void AddValue(DBHField field, Variant value)
    {
        if (field.FieldName == "Id")
            Id = (int)value;
        if (field.FieldName == "Class")
            Class = (string)value;
        // TODO: Implement when users dont want these default values

        Values.Add(field, value);
    }
}

[GlobalClass]
[Icon("res://addons/DBHero/dbhero.svg")]
public partial class DBHDatabase : Resource
{
    [Export]
    public Array<DBHField> Structure = new();
    [Export]
    public Array<DBHEntry> Entries = new();

    [Export]
    public string ClassName { get; set; }
    [Export]
    public string DbName { get; set; }
    [Export]
    public string NamespaceName { get; set; }

    public DBHDatabase()
    {
        ClassName = "";
        DbName = "db";
        NamespaceName = "";
        AddDefaultFields();
    }

    public DBHDatabase(string className = "MyClass", string dbName = "MyDB", string namespaceName = "MyGame", bool addDefaultFields = true)
    {
        ClassName = className;
        DbName = dbName;
        NamespaceName = namespaceName;
        if (addDefaultFields)
            AddDefaultFields();
    }

    public void AddDefaultFields()
    {
        Structure.Add(new DBHField()
        {
            FieldName = "Id",
            DefaultValue = "-1",
            FieldType = DBHFieldType.Integer,
            IsId = true,
            IsNullable = false,
        });

        Structure.Add(new DBHField()
        {
            FieldName = "Class",
            DefaultValue = "",
            FieldType = DBHFieldType.String,
            IsId = false,
            IsNullable = false,
            GenerateInCSharp = false,
        });
    }

    public Error LoadFromXML(string path)
    {
        var xml = new XmlDocument();
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var content = file.GetAsText();
        file.Close();

        xml.LoadXml(content);

        // Parse the XML
        // Get the root Db node
        XmlNode rootNode = null;
        string className = null, dbName = null, namespaceName = null;
        bool addDefaultFields = true;
        foreach (XmlNode c in xml.ChildNodes)
        {
            if (c.NodeType == XmlNodeType.Element && c.Name == "Db")
            {
                rootNode = c;
                if (c.Attributes != null)
                    foreach (XmlAttribute a in c.Attributes)
                    {
                        if (a.Name == "ClassName")
                            className = a.Value;
                        else if (a.Name == "DBName")
                            dbName = a.Value;
                        else if (a.Name == "Namespace")
                            namespaceName = a.Value;
                        else if (a.Name == "AddDefaultFields")
                            addDefaultFields = bool.Parse(a.Value);
                    }
                break;
            }
        }

        ClassName = className;
        NamespaceName = namespaceName;
        DbName = dbName;
        // TODO: Implement this as a field not a method
        //AddDefaultFields = addDefaultFields;

        // Check if rootNode exist
        if (rootNode == null)
        {
            GD.PrintErr($"Failed parsing database {path}, no root <Db/> node");
            return Error.Failed;
        }

        XmlNode structuresNode = null, entriesNode = null;
        // Check the structures node and entries node
        foreach (XmlNode c in rootNode.ChildNodes)
        {
            if (c.NodeType != XmlNodeType.Element) continue;
            if (c.Name == "Structures")
            {
                structuresNode = c;
                continue;
            }

            if (c.Name == "Entries")
            {
                entriesNode = c;
                continue;
            }
        }

        if (structuresNode == null || entriesNode == null)
        {
            GD.PrintErr($"Failed parsing database {path}, no Structures and/or Entries node");
            return Error.Failed;
        }

        // Parse the structures node
        foreach (XmlNode c in structuresNode.ChildNodes)
        {
            if (c.NodeType != XmlNodeType.Element) continue;
            if (c.Name != "Field") continue;
            if (c.Attributes == null) continue;
            var field = new DBHField
            {
                // Fill in name and type
                FieldName = c.Attributes["Name"].Value,
                FieldType = Enum.Parse<DBHFieldType>(c.Attributes["Type"].Value)
            };

            // Check if it's the ID field
            if (c.Attributes["IsId"] != null)
                field.IsId = c.Attributes["IsId"].Value == "true";

            // Add hint field
            if (c.Attributes["Hint"] != null)
                field.Hint = c.Attributes["Hint"].Value;

            if (field.FieldType == DBHFieldType.Resource)
            {
                // If resource, get the resource type
                if (c.Attributes["ResourceType"] != null)
                    field.ResourceType = c.Attributes["ResourceType"].Value;
            }

            Structure.Add(field);
        }

        // Parse the entries node
        foreach (XmlNode c in entriesNode.ChildNodes)
        {
            if (c.NodeType != XmlNodeType.Element) continue;
            if (c.Name != "Entry") continue;
            if (c.Attributes == null) continue;
            var entry = new DBHEntry();
            foreach (XmlAttribute a in c.Attributes)
            {
                // Find the correct field
                DBHField field = null;
                foreach (DBHField f in Structure)
                {
                    if (f.FieldName == a.Name)
                    {
                        field = f;
                        break;
                    }
                }

                if (field == null)
                {
                    GD.PrintErr($"Error parsing database {path}, field unknown: {a.Name}");
                    return Error.Failed;
                }

                // Check the field type and the provided content
                Variant fieldValue = new Variant();
                switch (field.FieldType)
                {
                    case DBHFieldType.String:
                        fieldValue = a.Value;
                        break;
                    case DBHFieldType.Boolean:
                        fieldValue = a.Value == "true";
                        break;
                    case DBHFieldType.Integer:
                        fieldValue = int.Parse(a.Value);
                        break;
                    case DBHFieldType.Float:
                        fieldValue = float.Parse(a.Value);
                        break;
                    case DBHFieldType.Resource:
                        fieldValue = a.Value; // Store just the path
                        break;
                    default:
                        GD.PrintErr($"Error parsing database {path}, attribute type not implemented: {field.FieldType} from attribute \"{field.FieldName}\"");
                        return Error.Failed;
                }

                // No errors, field exists, set the value
                entry.AddValue(field, fieldValue);
            }
            Entries.Add(entry);
        }


        return Error.Ok;
    }

    public void SaveToXML(string path)
    {
    }

    public string GenerateCSharp()
    {
        var strw = new StringBuilder();

        // Default Header
        strw.AppendLine("/// Generated by DBHero, DO NOT MANUALLY EDIT");
        strw.AppendLine("/// If broken, delete every lines in this file");
        strw.AppendLine("using Godot;");

        // Tab, change based on having namespace or not
        var tab = "";

        if (NamespaceName != "")
        {
            strw.AppendLine($"namespace {NamespaceName} {{"); // Namespace opening
            tab += "\t";
        }


        /////////////////////////////////////////////////
        ////////////// Generating Data Class ////////////
        /////////////////////////////////////////////////
        strw.AppendLine($"{tab}public class {ClassName} {{"); // Class opening
        foreach (var f in Structure)
        {
            if (f.GenerateInCSharp == false) continue;
            strw.AppendLine("\t" + tab + f.ToCSharpField());
        }
        strw.AppendLine($"{tab}}}"); // Class closing


        ///////////////////////////////////////////////
        ////////////// Adding each entries ////////////
        ///////////////////////////////////////////////
        strw.AppendLine($"{tab}// DBHero content here");
        strw.AppendLine($"{tab}public static class {DbName} {{");

        // Add all the entries into an array
        strw.Append($"{tab}\tpublic static {ClassName}[] Values = [");
        foreach (var e in Entries)
        {
            strw.Append($"{e.Class},");
        }
        strw.AppendLine("];");

        // Add each entries
        foreach (var e in Entries)
        {
            var line = $"{tab}\tpublic static {ClassName} {e.Class} = new {ClassName} {{\n";
            foreach (var v in e.Values)
            {
                if (v.Key.GenerateInCSharp == false) continue;
                var value = "";

                if (v.Key.FieldType == DBHFieldType.String)
                    value = $"\"{v.Value}\"";
                else if (v.Key.FieldType == DBHFieldType.Resource)
                    value = $"GD.Load<{v.Key.ResourceType}>(\"{v.Value}\")";
                else
                    value = (string)v.Value;
                // TODO: implement other special cases like Array or Dictionary

                line += $"{tab}\t\t{v.Key.FieldName} = {value},\n";
            }
            line += $"\n{tab}\t}};";
            strw.AppendLine(line);
        }
        strw.AppendLine($"{tab}}}");
        ////////////////////////////////////////////////

        if (NamespaceName != "")
            strw.AppendLine("}"); // Namespace closing
        return strw.ToString();
    }
}