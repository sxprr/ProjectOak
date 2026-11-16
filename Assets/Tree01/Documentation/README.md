# Tree 01 - Broadleaf Shade Trees (release 1.1.0; local candidate v12)

This free local candidate for release 1.1.0 contains three distinct stylized broadleaf trees: Young Upright, Mature Asymmetrical, and Wind-Swept. Each prefab uses three separately authored LOD meshes, textured bark, three foliage tones, and approximate root/trunk collision.

## Unity setup

- Tested with Unity 6000.3.25f1 in the Built-in Render Pipeline using the Standard shader.
- Import `Tree_Showroom_v12.unity` to review the native showcase, or drag a prefab from `Prefabs` into a scene.
- Models use meters, a ground-level root pivot, identity root rotation, unit scale, Y-up orientation, and +Z forward.
- UV0 and the packed UV2 lightmap channel are included in each FBX and preserved by Unity import; the QA report records channel lengths and checks.
- Each prefab uses LODGroup transition heights 0.34, 0.018, and 0.006. At lodBias 1 and 2 the LOD1-to-LOD2 transition is captured with original materials; a root flare sphere and two trunk capsules approximate static collision.
- The paired leaf surfaces render from both sides with the Standard shader; they do not rely on alpha cards.

## Included files

- Nine FBX source meshes (three LODs for each tree), bark color/normal textures, materials, prefabs, and one native showroom scene.
- The model checks and stated limitations are in `QA_v12.md`; imported mesh counts are in `Model_Statistics_v12.json` and `Model_Statistics_v12.txt`.
- `Documentation/Source/Tree01_Editable_Blender_Source_v12.zip` contains the editable Blender source, portable generator script, and source mesh statistics.

## Scope and limitations

The forms are stylized generic broadleaf trees. No species or historical accuracy is claimed. The pack contains no wind animation, SpeedTree integration, seasonal variants, terrain system, or dense-forest benchmark. Collision follows the trunk and root flare rather than the full foliage canopy.
