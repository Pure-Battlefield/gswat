// requestAnimationFrame() shim by Paul Irish
// http://paulirish.com/2011/requestanimationframe-for-smart-animating/
window.requestAnimFrame = (function() {
    return  window.requestAnimationFrame   ||
        window.webkitRequestAnimationFrame ||
        window.mozRequestAnimationFrame    ||
        window.oRequestAnimationFrame      ||
        window.msRequestAnimationFrame     ||
        function(callback) {
            window.setTimeout(callback, 1000 / 60);
        };
}());

_.extend(window, {
    pGSWAT: null
});

(function(window, document, $, _, yepnope, undefined) {
    pGSWAT = function () {
        // Object Variables
        this.view_instances         = {};
        this.model_instances        = {};
        this.collection_instances   = {};
        this.main_ele               = 'body';
        this.default_route          = 'main';
        this.CDN                    = '';
        this.files_loaded           = [];
        this.timers                 = {};
    };

    pGSWAT.prototype = {
        get_model: function(model_name,data){
            var models = this.model_instances;
            var model = (models[model_name])? models[model_name] : this.create_model(model_name);
            if(data){
                model.set(data);
            }
            return model;
        },

        get_view: function(view_name,model_name){
            var views = this.view_instances;
            var view = (views[view_name])? views[view_name] : this.create_view(view_name,model_name);
            return view;
        },

        get_collection: function(collection_name){
            var collections = this.collection_instances;
            var collection = (collections[collection_name])? collections[collection_name] : this.create_collection(collection_name);
            return collection;
        },

        create_model: function(model_name){
            var model = this.model_definitions[model_name];
            model = (model)? this.model_instances[model_name] = new model() : '';
            return model;
        },

        create_view: function(view_name,model_name){
            var view = this.view_definitions[view_name];
            var model = (!model_name || typeof model_name !== 'object')? this.get_model(model_name) : model_name;
            view = (view)? this.view_instances[view_name] = new view({model: model}) : '';
            return view;
        },

        create_collection: function(collection_name){
            var collection = this.collection_definitions[collection_name];
            collection = (collection)? this.collection_instances[collection_name] = new collection() : '';
            collection.name = collection_name;
            return collection;
        },

        load: function (files, callback) {
            $(this.main_ele).html(ich.tpl_loading());
            var last = _.last(files);
            yepnope([{
                load: files,
                callback: function (url) {
                    if (url === last) {
                        return callback();
                    }
                }
            }]);
        },

        set_options: function(options){
            var scope = this;
            $.each(options,function(index,option){
                scope[index] = option;
            });
        },

        init: function(options) {
            this.set_options(options);
            if (window.location.hash == undefined || window.location.hash == ''){
                window.location.hash = this.default_route;
            }
            this.active_router = new this.router();
            Backbone.history.start();
        }
    };

    // Global objects for our views/models/collections/events
    _.extend(pGSWAT.prototype, {
        view_definitions : {},
        model_definitions : {},
        collection_definitions : {}
    });
}(window, document, jQuery, _, yepnope));