

function main() {

	var canvas = document.getElementById('canvas');
	var ctx = canvas.getContext('2d');

	var grid = new Grid(10, 10, Node);
	var graph = grid.applyGraph(function(fromNode,toNode) {
		return fromNode.distance(toNode);
	})
	var nodes = grid.getNodesObject();
	var nodesArray = grid.getNodesArray();
	var startNode = nodesArray[Math.floor(Math.random() * nodesArray.length)];
	var endNode = nodesArray[Math.floor(Math.random() * nodesArray.length)];

	var path = graph.shortestPath(startNode.valueOf(), endNode.valueOf());

	nodesArray.forEach(function(node){
		node.draw(ctx, 'black');
	});
	path.forEach(function(node) {
		nodes[node].draw(ctx, 'red', 2,2);
	});
}

document.addEventListener('DOMContentLoaded', main);