describe("Main Object", function() {
	var PBF = new window.GSWAT();
	_.extend(window,{PBF:PBF});

	describe("should create", function(){
		it("a new model instance",function(){
			PBF.model_definitions.test_model = Backbone.Model.extend({});
			var test_model = PBF.get({model:{name:'test_model'}});
			expect(test_model instanceof Backbone.Model).toBe(true);
		});

		it("a new view instance",function(){
			PBF.view_definitions.test_view = Backbone.View.extend({});
			var test_view = PBF.get({view:{name:'test_view'}});
			expect(test_view instanceof Backbone.View).toBe(true);
		});

		it("a new collection instance",function(){
			PBF.collection_definitions.test_collection = Backbone.Collection.extend({});
			var test_collection = PBF.get({collection:{name:'test_collection'}});
			expect(test_collection instanceof Backbone.Collection).toBe(true);
		});
	});
});