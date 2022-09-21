﻿using System;
using System.Linq;
using GBG.AnimationGraph.Editor.Blackboard;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.Editor.ViewElement;
using GBG.AnimationGraph.GraphData;
using GBG.AnimationGraph.Parameter;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.AnimationGraph.Editor.GraphEditor
{
    public partial class AnimationGraphEditorWindow
    {
        private void CreateBlackboardPanel()
        {
            var splitter = new DoubleSplitterColumnView(new Vector2(20, 80));
            _layoutContainer.LeftPane.Add(splitter);

            CreateBlackboardParamView(splitter);

            CreateBlackboardGraphView(splitter);
        }


        #region Parameter

        private Toolbar _blackboardParamToolbar;

        private ListView _paramListView;


        private void CreateBlackboardParamView(DoubleSplitterColumnView blackboardSplitter)
        {
            // Toolbar
            _blackboardParamToolbar = new Toolbar { style = { justifyContent = Justify.SpaceBetween, } };
            blackboardSplitter.UpPane.Add(_blackboardParamToolbar);

            // Parameter label
            var paramPaneLabel = new Label("Parameters");
            _blackboardParamToolbar.Add(paramPaneLabel);

            // Add parameter button
            var addParamButton = new ToolbarButton { text = "+" };
            addParamButton.clickable ??= new Clickable((Action)null);
            addParamButton.clickable.clickedWithEventInfo += OnAddParamButtonClicked;
            _blackboardParamToolbar.Add(addParamButton);

            // Parameter list view
            _paramListView = new ListView
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = 24,
                makeItem = MakeParamListItem,
                bindItem = BindParamListItem,
                selectionType = SelectionType.Single,
            };
            _paramListView.itemIndexChanged += OnParamIndexChanged;
            blackboardSplitter.UpPane.Add(_paramListView);
        }

        private void OnAddParamButtonClicked(EventBase evt)
        {
            var menu = new GenericDropdownMenu();
            menu.AddItem("Float", false, () =>
            {
                Parameters.Add(new ParamInfo(GuidTool.NewGuid(), $"Float_{GuidTool.NewUniqueSuffix()}",
                    ParamType.Float, 0));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });
            menu.AddItem("Integer", false, () =>
            {
                Parameters.Add(new ParamInfo(GuidTool.NewGuid(), $"Int_{GuidTool.NewUniqueSuffix()}",
                    ParamType.Int, 0));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });
            menu.AddItem("Bool", false, () =>
            {
                Parameters.Add(new ParamInfo(GuidTool.NewGuid(), $"Bool_{GuidTool.NewUniqueSuffix()}",
                    ParamType.Bool, 0));
                _paramListView.RefreshItems();
                hasUnsavedChanges = true;
            });

            var menuPos = Vector2.zero;
            if (evt is IMouseEvent mouseEvt)
            {
                menuPos = mouseEvt.mousePosition;
            }
            else if (evt is IPointerEvent pointerEvt)
            {
                menuPos = pointerEvt.position;
            }
            else if (evt.target is VisualElement visualElem)
            {
                menuPos = visualElem.layout.center;
            }

            menu.DropDown(new Rect(menuPos, Vector2.zero), _blackboardParamToolbar);
        }

        private VisualElement MakeParamListItem()
        {
            var paramField = new ParamField();
            paramField.OnParamValueChanged += OnParamValueChanged;
            paramField.OnWantsToRenameParam += OnWantsToRenameParam;

            return paramField;
        }

        private void BindParamListItem(VisualElement listItem, int index)
        {
            var paramField = (ParamField)listItem;
            var paramInfo = Parameters[index];
            paramField.SetParamInfo(paramInfo);
        }

        private void OnParamIndexChanged(int from, int to)
        {
            hasUnsavedChanges = true;
        }

        private void OnParamValueChanged(ParamInfo _)
        {
            hasUnsavedChanges = true;
        }

        private void OnWantsToRenameParam(ParamInfo paramInfo)
        {
            var conflictingNames = from param in _graphAsset.Parameters
                where !param.Name.Equals(paramInfo.Name)
                select param.Name;
            RenameWindow.Open(paramInfo.Name, conflictingNames, (oldName, newName) =>
            {
                if (oldName.Equals(newName)) return;
                paramInfo.EditorSetName(newName);

                _paramListView.RefreshItems();

                hasUnsavedChanges = true;
            });
        }

        #endregion


        #region Graph

        private Toolbar _blackboardGraphToolbar;

        private ListView _graphListView;


        private void CreateBlackboardGraphView(DoubleSplitterColumnView blackboardSplitter)
        {
            // Toolbar
            _blackboardGraphToolbar = new Toolbar { style = { justifyContent = Justify.SpaceBetween, } };
            blackboardSplitter.DownPane.Add(_blackboardGraphToolbar);

            // Graph label
            var graphPaneLabel = new Label("Graphs");
            _blackboardGraphToolbar.Add(graphPaneLabel);

            // Add graph button
            var addGraphButton = new ToolbarButton { text = "+" };
            addGraphButton.clickable ??= new Clickable((Action)null);
            addGraphButton.clickable.clickedWithEventInfo += OnAddGraphButtonClicked;
            _blackboardGraphToolbar.Add(addGraphButton);

            // Graph list view
            _graphListView = new ListView
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                fixedItemHeight = 24,
                makeItem = MakeGraphListItem,
                bindItem = BindGraphListItem,
                selectionType = SelectionType.None,
            };
            _graphListView.itemIndexChanged += OnGraphIndexChanged;
            blackboardSplitter.DownPane.Add(_graphListView);
        }

        private void OnAddGraphButtonClicked(EventBase evt)
        {
            var menu = new GenericDropdownMenu();
            menu.AddItem("Mixer Graph", false, () =>
            {
                Graphs.Add(new GraphData.GraphData(GuidTool.NewGuid(),
                    $"BlendGraph_{GuidTool.NewUniqueSuffix()}",
                    GraphType.Mixer));
                _graphListView.RefreshItems();
                hasUnsavedChanges = true;
            });
            menu.AddItem("State Machine Graph", false, () =>
            {
                Graphs.Add(new GraphData.GraphData(GuidTool.NewGuid(),
                    $"StateMachineGraph_{GuidTool.NewUniqueSuffix()}",
                    GraphType.StateMachine));
                _graphListView.RefreshItems();
                hasUnsavedChanges = true;
            });

            var menuPos = Vector2.zero;
            if (evt is IMouseEvent mouseEvt)
            {
                menuPos = mouseEvt.mousePosition;
            }
            else if (evt is IPointerEvent pointerEvt)
            {
                menuPos = pointerEvt.position;
            }
            else if (evt.target is VisualElement visualElem)
            {
                menuPos = visualElem.layout.center;
            }

            menu.DropDown(new Rect(menuPos, Vector2.zero), _blackboardGraphToolbar);
        }


        private VisualElement MakeGraphListItem()
        {
            var graphField = new GraphField();
            graphField.OnWantsToRenameGraph += OnWantsToRenameGraph;
            graphField.OnWantsToOpenGraph += OnWantsToOpenGraph;

            return graphField;
        }

        private void BindGraphListItem(VisualElement listItem, int index)
        {
            var graphField = (GraphField)listItem;
            graphField.style.flexGrow = 1;

            var graphData = Graphs[index];
            graphField.SetGraphData(graphData, graphData.Guid != _graphAsset.RootGraphGuid);
        }

        private void OnGraphIndexChanged(int from, int to)
        {
            hasUnsavedChanges = true;
        }

        private void OnWantsToRenameGraph(GraphData.GraphData graphData)
        {
            var conflictingNames = from graph in _graphAsset.Graphs
                where !graph.Name.Equals(graphData.Name)
                select graph.Name;
            RenameWindow.Open(graphData.Name, conflictingNames, (oldName, newName) =>
            {
                if (oldName.Equals(newName)) return;
                graphData.Name = newName;

                _graphListView.RefreshItems();

                RefreshGraphViewBreadcrumbLabel(graphData.Guid, graphData.Name);

                hasUnsavedChanges = true;
            });
        }

        private void OnWantsToOpenGraph(GraphData.GraphData graphData)
        {
            OpenGraphView(graphData.Guid, true);
        }

        #endregion
    }
}