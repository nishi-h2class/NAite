$(function () {

	// パラメータをチェックしてvalueにセット
	var paramArray = naite.getParam() || {};
	console.log(paramArray);

	naite.vuemodel = new Vue({
		el: '#my-app',
		data: {
			firstName: null,
			lastName: null,
			department: null,
			loginId: null,
			email: null,
			authority: 'user',
			isNotifyUpdateData: false
		},
		mounted: function () {
			naite.vuemodel = this;
			if ($.cookie('authority') != 'admin') {
				// ホームへ
			}
		},
		methods: {
			doSubmit: async function (event) {
				var validator = this.$validator;
				swal({
					title: "登録しますか？",
					text: "",
					type: "warning",
					showCancelButton: true,
					confirmButtonClass: "btn btn-teal",
					confirmButtonText: "はい",
					cancelButtonClass: "btn btn-default",
					cancelButtonText: "キャンセル"
				}).then(function (isConfirm) {
					if (isConfirm.value) {
						validator.validateAll()
							.then((result) => {
								if (result) {

									// 二重クリック防止
									naite.loading(true);

									// パラメータ
									var item = {
										LoginId: naite.vuemodel.$data.loginId,
										FirstName: naite.vuemodel.$data.firstName,
										LastName: naite.vuemodel.$data.lastName,
										Department: naite.vuemodel.$data.department,
										Email: naite.vuemodel.$data.email,
										Authority: naite.vuemodel.$data.authority,
										IsNotifyUpdateData: naite.vuemodel.$data.isNotifyUpdateData
									}
									console.log(item);

									naite.post(naite.apiUrls.users, item)
										.done(function (data) {
											console.log(data);
											// 二重クリック解除
											naite.loading(false);
											swal({
												title: "登録が完了しました",
												text: "",
												type: "success"
											}).then(function () {
												naite.redirect(naite.urls.users);
											});
										})
										.fail(function (error) {
											console.log(error);
											var msg = naite.handleError(error);
											if (msg) {
												swal(msg, "", "error");
											}
										});
								}
								else {
									swal.closeModal();
								}
							});
					}
				});
			}
		}
	});
});
