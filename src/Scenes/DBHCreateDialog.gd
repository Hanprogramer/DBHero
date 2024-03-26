@tool
extends Window

var gen_class : bool = false
var dbname : String
var namespacename : String
var classname : String
var add_defaultfields: bool = true

@export var le_dbname: LineEdit
@export var le_namespacename: LineEdit
@export var le_classname: LineEdit


signal create(path, dbname, namespacename, 
			  gen_class, classname, add_defaultfields)

var fd : EditorFileDialog

func _ready():
	fd = EditorFileDialog.new()
	fd.access = EditorFileDialog.ACCESS_RESOURCES
	fd.file_mode = EditorFileDialog.FILE_MODE_SAVE_FILE
	# needed so the script is not directly freed after the run function. 
	# Would disconnect all signals otherwise
	fd.set_meta("_created_by", self)  
	fd.add_filter("*.dbhero, *.dbh", "DBH Database")
	fd.size = Vector2(900,600)
	fd.title = "Create DBH Database"
	fd.initial_position = Window.WINDOW_INITIAL_POSITION_CENTER_SCREEN_WITH_MOUSE_FOCUS
	
	#var viewport = EditorInterface.get_editor_main_screen().get_viewport()
	#viewport.call_deferred("add_child",fd)
	add_child(fd)
	
	fd.connect("file_selected", on_file_selected)

func _on_btn_cancel_pressed():
	hide()

func _on_btn_create_pressed():
	dbname = le_dbname.text
	namespacename = le_namespacename.text
	classname = le_classname.text
	
	if dbname == "":
		dbname = "MyDB"
	
	fd.popup_centered()


func _on_cb_gen_class_toggled(toggled_on):
	gen_class = toggled_on
	le_classname.editable = toggled_on
	le_namespacename.editable = toggled_on

func on_file_selected(path: String):
	create.emit(path, dbname, namespacename, gen_class, classname, add_defaultfields)
	hide()

func _on_close_requested():
	hide()

func _on_cb_default_fields_toggled(toggled_on):
	add_defaultfields = toggled_on
