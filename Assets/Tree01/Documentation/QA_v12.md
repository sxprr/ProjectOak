# Tree 01 - Broadleaf Shade Trees: QA

Candidate identity: Tree 01, product 192541, repair draft 1520916; release 1.1.0, Free. This is a local successor candidate. It has not been uploaded or published; independent review of the final Unity package is pending.

## Included product content

Three modeled stylized forms are provided: Young Upright, Mature Asymmetrical, and Wind-Swept. Each has three authored LOD meshes (nine FBX files total), one prefab with an LODGroup and three approximate static trunk/root colliders, a bark material and two 512 x 512 bark textures, and a native Unity showroom scene. The package also includes the editable Blender 5.1.2 source archive, source statistics, Unity import statistics, README, and this QA report.

Models use meters, Y-up, +Z forward, and a ground-level origin. Prefab root transform is zero position, identity rotation, unit scale. Bark and foliage are separate meshes. Leaf forms are intentionally thin, two-sided surfaces; they are not closed solids. UV0 and a packed UV2 channel are present and preserved by Unity import. No non-overlapping UV2 claim is made.

## Geometry and Unity import checks

A fresh Blender 5.1.2 audit of the sanitized editable release source and all nine exported FBXs found each of the nine bark meshes triangulated as a single closed, positive-volume component, with zero boundary edges, non-manifold edges, inconsistent edges, loose vertices, duplicate faces, or degenerate faces. The audit found zero non-adjacent bark-triangle intersections. The release source contains the 18 intended tree mesh objects plus its QA ground; the hidden experimental diagnostic meshes are excluded. Both bark images are packed into the editable source.

Unity 6000.3.25f1 imported all nine FBXs in the Built-in Render Pipeline using Standard materials. The build produced 18 imported mesh records (bark and foliage for each tree/LOD). Every record has UV0 and UV2 arrays matching its vertex count and zero degenerate triangles under the unchanged Unity test (area-vector squared below 1e-20 m^4). The final build log contains no model-face discard or self-intersection warnings. Foliage's intentional open edges are excluded from the bark watertight-shell test. A clean reimport of the exported package into a new Unity 6000.3.25f1 project also passed: 9 FBXs, 18 meshes, 3 prefabs, all UV/degenerate and LOD/collider/root checks, and a showroom scene with all three trees. The producer reimport record is CleanReimport-v12.json.

The v12 local junction relaxation changes only vertex positions around five previously confirmed LOD0 branch-junction crossings; it preserves topology and face counts. The full nine-bark audit was rerun after the change.

## Imported triangle counts and LOD reduction

Counts below aggregate both imported meshes (bark and foliage) per tree. Reduction percentages compare each lower LOD with that tree's LOD0.

| Tree | LOD0 | LOD1 | LOD1 reduction | LOD2 | LOD2 reduction |
|---|---:|---:|---:|---:|---:|
| Young Upright | 77,444 | 26,578 | 65.7% | 11,900 | 84.6% |
| Mature Asymmetrical | 96,802 | 33,050 | 65.9% | 16,300 | 83.2% |
| Wind-Swept | 78,398 | 27,340 | 65.1% | 13,842 | 82.3% |

Each prefab uses LOD transition heights 0.34, 0.018, and 0.006. Unity automatic LOD selection was tested around the LOD1-to-LOD2 transition with marker selection counts and separate renders using the original materials. At lodBias 1, the test selected LOD1 at screen height 0.019 and LOD2 at 0.017. At lodBias 2, it selected LOD1 at 0.010 and LOD2 at 0.008. The rendered evidence and selector readings are in Unity_Automatic_LOD_Observations_v12.txt and the paired AutoLOD_Normals captures. These tests verify selection at the chosen thresholds; they are not a performance benchmark.

## Visual inspection and limitations

Unity produced a native three-tree showroom render on a matte ground plane with contact shadows, plus front, back, side, top, underside, root close-up, branch close-up, and forced-LOD0/1/2 views for each form. These are actual Unity camera renders, not the concept image. The source archive also retains the editable Blender scene and portable generator.

The trees are generic stylized broadleaf assets; no species or historical accuracy is claimed. This is static geometry with simple approximate trunk/root collision. It includes no wind animation, SpeedTree integration, seasonal variants, terrain system, or dense-forest performance benchmark. LOD transitions do not cross-fade.