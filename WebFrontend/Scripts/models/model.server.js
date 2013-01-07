(function(window, $, _) {
    var server_model = Backbone.Model.extend({
        defaults : {
            ServerIP: '',
            Password: '',
            ServerPort: '',
            OldPassword: '',
            server_settings_url: '/api/values/setserverinfo',
            settings_success: 3
        },

        initialize: function () {
            //
        },

        update_settings: function (data) {
            var model = this;
            $.ajax({
                type: 'POST',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Content-type', 'application/json')
                },
                contentType: 'application/json; charset=utf-8',
                url: model.get('server_settings_url'),
                data: JSON.stringify(data), //TODO: No likey JSON string, this should be a standard POST..
                dataType: 'json',
                success: function (success) {
                    console.log(model);
                    model.set({ 'settings_success': 1 });
                    var chat_settings = PBF.get_model('chat_model');
                    console.log(chat_settings);
                    chat_settings.set({ 'server_set': true });
                    chat_settings.set_interval();
                },
                error: function (error) {
                    model.set({'settings_success': 0});
                }
            });
        }
    });

    _.extend(window.GSWAT.prototype.model_definitions, { server_model: server_model });
}(window, jQuery, _));