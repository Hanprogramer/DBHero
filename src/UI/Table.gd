extends Control

var rows = []
var headers: Array = []

@export var header_cont : HBoxContainer
@export var items_cont : VBoxContainer

func add_header(title: String, fill=false, ratio=1.0):
	headers.append({
		"title": title,
		"fill": fill,
		"ratio": ratio,
		"initial_ratio": ratio
	})
	update_headers()

func update_headers():
	# Clear the headers
	for c in header_cont.get_children():
		c.queue_free()
	
	# Add the new headers
	var i = 0
	for h in headers:
		var header = Label.new()
		header.text = h["title"]
		if h["fill"]:
			header.size_flags_horizontal = Control.SIZE_FILL | Control.SIZE_EXPAND
		header.size_flags_stretch_ratio = h["ratio"]
		header_cont.add_child(header)
