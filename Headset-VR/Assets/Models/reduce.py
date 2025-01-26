import bpy
import os

# Settings
input_folder = "Text"
output_folder = "Text-conv"
reduction_ratio = 0.1  # 50% reduction, adjust as needed

# Ensure output folder exists
if not os.path.exists(output_folder):
    os.makedirs(output_folder)

# Get all GLB files in the input folder
glb_files = [f for f in os.listdir(input_folder) if f.endswith('.GLB')]

# Function to reduce the mesh complexity
def reduce_mesh(obj, ratio):
    # Switch to object mode
    bpy.ops.object.mode_set(mode='OBJECT')
    # Select the object
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    # Add a decimate modifier
    decimate = obj.modifiers.new(name="Decimate", type='DECIMATE')
    decimate.ratio = ratio
    # Apply the modifier
    bpy.ops.object.modifier_apply(modifier="Decimate")
    obj.select_set(False)

# Process each file
for glb_file in glb_files:
    input_path = os.path.join(input_folder, glb_file)
    output_path = os.path.join(output_folder, glb_file)
    
    # Clear the scene
    bpy.ops.wm.read_factory_settings(use_empty=True)
    
    # Import the GLB file
    bpy.ops.import_scene.gltf(filepath=input_path)
    
    # Reduce all meshes in the scene
    for obj in bpy.context.scene.objects:
        if obj.type == 'MESH':
            reduce_mesh(obj, reduction_ratio)
    
    # Export the reduced GLB file
    bpy.ops.export_scene.gltf(filepath=output_path, export_format='GLB')
    print(f"Processed and saved: {output_path}")

print("Batch processing completed!")
