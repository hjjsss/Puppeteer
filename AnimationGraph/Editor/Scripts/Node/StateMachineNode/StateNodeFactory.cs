﻿using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Editor.Utility;
using GBG.AnimationGraph.NodeData;
using UnityEngine;

namespace GBG.AnimationGraph.Editor.Node
{
    public static class StateNodeFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> _nodeToDataType = new Dictionary<Type, Type>
        {
            { typeof(StateNode), typeof(StateNodeData) },
        };

        private static readonly IReadOnlyDictionary<Type, Type> _dataToNodeType = new Dictionary<Type, Type>
        {
            { typeof(StateNodeData), typeof(StateNode) },
        };


        public static IEnumerable<Type> GetStateNodeTypes() => _nodeToDataType.Keys;

        public static StateNode CreateNode(AnimationGraphAsset graphAsset, Type nodeType, Vector2 position)
        {
            var nodeDataType = _nodeToDataType[nodeType];
            var nodeData = (StateNodeData)Activator.CreateInstance(nodeDataType, GuidTool.NewGuid());
            nodeData.EditorPosition = position;

            return CreateNode(graphAsset, nodeData);
        }

        public static StateNode CreateNode(AnimationGraphAsset graphAsset, StateNodeData nodeData)
        {
            var nodeType = _dataToNodeType[nodeData.GetType()];
            var node = (StateNode)Activator.CreateInstance(nodeType, graphAsset, nodeData);

            if (string.IsNullOrEmpty(nodeData.StateName))
            {
                nodeData.StateName = "State_" + GuidTool.NewUniqueSuffix();
            }

            node.title = nodeData.StateName;

            return node;
        }
    }
}