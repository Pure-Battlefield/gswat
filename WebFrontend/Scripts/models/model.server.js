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
            console.log(data);
            $.ajax({
                type: 'POST',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Content-type', 'application/json')
                },
                contentType: 'application/json; charset=utf-8',
                url: model.get('server_settings_url'),
                data: data,
                dataType: 'json',
                success: function () {
                    model.set({'settings_success':1});
                }
            });
        }
    });

    _.extend(window.GSWAT.prototype.model_definitions, { server_model: server_model });
}(window, jQuery, _));