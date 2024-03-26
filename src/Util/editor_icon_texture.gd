@tool
class_name EditorIconTexture
extends ImageTexture

@export var icon_name : String:
	set(value):
		icon_name = value
		_update()

@export var icon_texture: Texture2D

func _init():
	_update()

func _update():
	icon_texture = EditorInterface.get_editor_main_screen().get_theme_icon(icon_name, "EditorIcons")
	#if icon_texture != null:
		#create_from_image(icon_texture.get_image())
