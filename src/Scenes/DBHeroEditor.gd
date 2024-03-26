@tool
extends Control

@export var create_dialog : Window


func _on_btn_create_db_pressed():
	create_dialog.popup()


func _on_dbh_create_dialog_create(path, dbname, namespacename, gen_class, classname, add_defaultfields):
	print("Creating in: "  + path)
	#DBHAutoload.CreateNewDB()#.CreateNewDB(path,dbname,namespacename,gen_class,classname, add_defaultfields)
	#DBHAutoload.CreateNewDB()


func _on_btn_save_db_pressed():
	pass # Replace with function body.
