# Perlin check

Handy editor-based tool for combining simplex noises via building formula in the graph editor.
Can be used to gain understanding about how different formula members do impact on the resulting noise map.

**Supported unity3d version:** 2021.3.1f1

***Currently supports only dark UI theme!***

  | ![](Docs/img0.gif?raw=true"") |
  |:--:|
  | *Change parameter and watch how it reflects on the generated mesh* |

## Usage
- Create the "**ComplexFormula**" asset from the context menu in the project window:

  | ![](Docs/img3.png?raw=true"") |
  |:--:|
  | *Asset creation* |

- In the scene heirarchy window, select "**Terrain**" GO under the "**Root**" and drag-n-drop your ComplexFormula asset in the appropriate field in the "**Formula**" component:

  | ![](Docs/img4.png?raw=true"") |
  |:--:|
  | *Bind your asset to the scene object that will reflect this asset's state* |

    The "**Formula**" and the "**Terrain test**" components are responsible for mesh generation as representation of the formula object calculation results, in the scene view (all scripts executes in designer mode):

  | ![](Docs/img5.png?raw=true"") |
  |:--:|
  | *Green areas are cut-off (data value exceeds the selected cap value)* |

  All parameters of the "**Terrain test**" do not change the formula, they are like preview options:
  * "**Map size**": defines the preview mesh size (width and height)
  * "**Height scale**": formula output range supposed to be in [0 .. 1] range and it can result in the flat-looking mesh. This parameter simply multiplies the height of the mesh.
  * "**Cut level**": this value limits maximum mesh height so affected areas will be filled with defined color
  * "**Cut color**": color which be used to fill the cut areas with

  All parameters change will be reflected on the mesh immediately.

  As you could notice, mesh has been painted in the black-to-white colors that helps to understand the data value for every vertex. Colors are shown not in the smooth 0 to 1 transition, but
    with 10% step division. This is only the visual effect, the real data values are reflected by the mesh height.

- Select the created "**ComplexFormula**" asset and press "**Edit**" button in the inspector window. The graph editor window appears:

  | ![](Docs/img6.png?raw=true"") |
  |:--:|
  | *ComplexFormula editor window* |

- When the node configuring is over, press the "**Show code**" button - it shows the popup window that contains selectable C# code for the current node configuration:

  | ![](Docs/img11.png?raw=true"") |
  |:--:|
  | *[Out] node has no links so its input always will be 0. Generated mesh also will be filled with black color.* |

## Node editor window
  Node editor has three areas:
  - Left panel with operator buttons, "**Show code**" button and usage tips
  - Right panel area with node graph
  - Bottom panel with node graph area zoom controls

  "**Operators**" block contains the buttons those let you add new nodes in the graph area.
  Graph node area has the next mouse control actions:
  - Panning with the middle mouse button
  - Zooming by mouse wheel scrolling

**Note:** *using zoom values other than 100% can result in some weird UI drawing artifacts - as example, mouse cursor icon will change unpredictably. Unfortunately, I cannot do anything with this 
  behavior, because Unity's IMGUI has no official support for the UI scaling functionality (I had to use some GUI.matrix manipulations to implement zooming, but it also has unpredictable side effects).*

  Newly created ComplexFormula contains only three nodes:
  * X [In]
  * Y [In]
  * [Out]

  Those nodes have special blue and green title. Common nodes have gray title.

  Those nodes represent formula function arguments (x, y) and the return value.
  After pressing one of the operator buttons, new node will be created in the middle of the screen:

  | ![](Docs/img7.png?raw=true"") |
  |:--:|
  | *"**Norm01**" node* |

  Every node have/may have common elements:
  - Operator name (with the index number inside of the brackets)
  - "**?**" help button (shows popup with short explanation about the current node usage)
  - Input sockets
  - Output sockets
  - Main area with the configurable parameters

  There are few things you need to know about the nodes appearance and behavior:
  - One node represents the single formula equation/function step

    | ![](Docs/img8.png?raw=true"") |
    |:--:|
    | *All kinds of the nodes are shown together (all existing at this moment)* |

  - You can connect different nodes by clicking on the socket and dragging green "wire" connection onto the another socket:

    | ![](Docs/img9.png?raw=true"") |
    |:--:|
    | *Green "wire" connection* |

  - You cannot connect two sockets of the same type (i.e. you cannot connect input-to-input, only input-to-output).
  - You cannot make the socket connection that results in recursive dependency.
  - Every input socket has the name that shows as a tooltip by hovering it with the mouse.
  - Input sockets are the representation of the operator/function input arguments, but every operator/function result is always single. And the node output value will be 
    passed through the output socket. But sometimes you want to reuse this "return" value and pass it to the mulptiple nodes - this is why "+" output socket 
    exists: when you connect the "+" socket, there are always new "+" socket wil be added so you always can connect more inputs to it. Removing the link also removes its output socket.
  - All input sockets those has no connection, their value is binded to zero by default. It means, if you has no input connection with the "**[Out]**" node, 
    resulting mesh will be flat and filled with black:

    | ![](Docs/img10.png?raw=true"") |
    |:--:|
    | *This rule has one exception: the "**Simplex01**" third input socket binding results into using the 3d noise formula instead.* |

  - Any node connection or configurable parameter changes will be instantly reflected in the mesh - this is the main target of this project. You 
      change value and watch how it reflects on the result without waiting for code recompilation/assembly reloading.
  - All the nodes except input and output ones can be removed by pressing on it with the right mouse button - and pressing "Remove" in the context menu:

    | ![](Docs/img12.png?raw=true"") |
    |:--:|
    | *Right-click on the node shows context menu* |

  - Nodes can be dragged by holding its title area. Position is snapped to the grid.
  - The node you click becomes selected. Selected nodes has yellow-shining frame:

    | ![](Docs/img13.png?raw=true"") |
    |:--:|
    | *"**Number**" node is selected* |

      Clicking with pressed ctrl key removes clicked node from the selection.
  - You can select multiple nodes by holding the shift key or use the area selection frame:

    | ![](Docs/img14.png?raw=true"") |
    |:--:|
    | *Two green nodes will be selected* |

  - Holding ctrl or shift key "add to the selection/remove from the selection" behavior also shared for selection frame. It also shows 
      "will be added", "will be removed" and "already selected" nodes with the different color:

      | ![](Docs/img15.png?raw=true"") |
      |:--:|
      | *Green means "will be added to the current selection". Orange means "already selected"* |

      | ![](Docs/img16.png?raw=true"") |
      |:--:|
      | *Red means "will be removed from the current selection"* |

  - Multiple selection feature can be handy when you need to remove or move multiple nodes in the same time. Unfortunately, at this moment the node copy-paste feature is not supported (yet? who knows).
  - Nodes those has no connection (directly or indirectly) to the "**[Out]**" node do not participate in the formula calculation or the code generation process. The 
      formula calculation process begins from the "**[Out]**" node, trying to calculate all it dependencies recursively. It just ignores the nodes those does not participate in the argument-dependency chain.

## Formula code generation
  There is two ways to use the formula:
  - Press "**Show code**" button and copy-paste it somewhere. This code reflects the current node configuration. Variable names in the generated code have "_"+number suffix - the number reflects the
      node index showing in the node title in the brackets:

      | ![](Docs/img17.png?raw=true"") |
      |:--:|
      | *Each node that participate in final result calculation has appropriate variable in the generated code. Its name also contains the number, so you can understand how node turns into a code* |

  - Use "**ComplecFormula**" asset directly and call its "**Calculate**" method:

    | ![](Docs/img18.png?raw=true"") |
    |:--:|
    | *This way is used to reflect the changes in the editor in designer time (to calculate the preview mesh), but it will be a lot slower than the previous one (not optimized, no node calculation results caching, has a lot of recursive calls, etc.)* |



