(function(window, $, _) {
    var server_model = Backbone.Model.extend({
        defaults : {
            'ServerIP'              : '',
            'Password'              : '',
            'ServerPort'            : '',
            'OldPassword'           : '',
            'server_settings_url'   : '/api/values/setserverinfo',
            'settings_success'      : 3
        },

        initialize: function () {
            //
        },

        update_settings: function (data) {
            var model = this;
            var chat_settings = PBF.get_model('chat_model');
            $.ajax({
                type: 'POST',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Content-type', 'application/json')
                },
                contentType: 'application/json; charset=utf-8',
                url: model.get('server_settings_url'),
                data: JSON.stringify(data), //TODO: Change this to a standard POST maybe?
                dataType: 'json',
                success: function (success) {
                    model.set({ 'settings_success': 1 });
                    chat_settings.set({ 'server_set': true });
                },
                error: function (error) {
                    model.set({ 'settings_success': 0 });
                    chat_settings.set({ 'server_set': false });
                }
            });
        }
    });

    _.extend(window.GSWAT.prototype.model_definitions, { server_model: server_model });
}(window, jQuery, _));