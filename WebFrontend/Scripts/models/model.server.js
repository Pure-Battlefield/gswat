(function(window, $, _) {
    var server_model = Backbone.Model.extend({
        defaults : {
            'ServerIP'              : '',
            'Password'              : '',
            'ServerPort'            : '',
            'OldPassword'           : '',
            'server_settings_url'   : '/api/serverInfo'
        },

		get_settings: function(){
			var model = this;
			$.ajax({
				type: 'GET',
				beforeSend: function (xhr) {
					xhr.setRequestHeader('Content-type', 'application/json')
				},
				contentType: 'application/json; charset=utf-8',
				url: model.get('server_settings_url'),
				dataType: 'json',
				success: function (data) {
					var settings = {};
					_.each(data,function(value,key){
						settings[key] = value;
					});
					model.set(settings);
				},
				error: function (error) {
					PBF.alert({type:'error',title:'An error occurred',message:error});
				}
			});
		},

        update_settings: function (data) {
            var model = this;
            var chat_settings = PBF.get({model:{name:'chat_model'}});
            $.ajax({
                type: 'PUT',
				beforeSend: function (xhr) {
					xhr.setRequestHeader('Content-type', 'application/json')
				},
				contentType: 'application/json; charset=utf-8',
                url: model.get('server_settings_url'),
                data: JSON.stringify(data),
                dataType: 'json',
                success: function (success) {
					PBF.alert({type:'success',title:'Success!',message:'Settings saved!'});
                    chat_settings.set({'server_set':true});
                },
                error: function (error) {
					PBF.alert({type:'error',title:'An error occurred',message:error});
                    chat_settings.set({'server_set':false});
                }
            });
        }
    });

    _.extend(window.GSWAT.prototype.model_definitions, { server_model: server_model });
}(window, jQuery, _));