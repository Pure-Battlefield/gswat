(function(window, $, _, ich) {
	_.extend(window.GSWAT.prototype.view_definitions, {
		map_rotation: Backbone.View.extend({
			id: 'map-rotation',

			initialize: function () {
				this.model.on("change", this.render, this);
			},

			render: function () {
				this.$el.append(ich.tpl_map_rotation(this.model.toJSON()));
			}
		})
	});
}(window, jQuery, _, ich));