$(function () {

    // メッセージ表示
    naite.checkParam();
    // ページ
    naite.initPagination(getPage);
    // パラメータをチェックしてvalueにセット
    var paramArray = naite.getParam() || {};

    naite.vuemodel = new Vue({
        el: '#my-app',
        data: {
            fields: [],
            itemRows: [],
            originalData: [],
            changedData: [],
            pageSize: paramArray.pageSize || 100,
            pageNumber: paramArray.pageNumber || 1,
            sortKey: paramArray.sortKey || null, // 現在のソートキー
            sortOrder: paramArray.sortOrder || null // 現在のソート順 ('asc' or 'desc')
        },
        mounted: function () {
            naite.vuemodel = this;
            this.getFields();
        },
        methods: {
            getFields: function () {
                naite.get(naite.apiUrls.itemFields, null)
                    .done(function (data) {
                        console.log(data);
                        naite.vuemodel.$data.fields = data;
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].isSearch == true) {
                                var searchField = {
                                    id: data[i].id,
                                    name: data[i].name,
                                    type: data[i].type,
                                    text: null,
                                    intStart: null,
                                    intEnd: null,
                                    dateStart: null,
                                    dateEnd: null
                                };
                                switch (searchField.type) {
                                    case "int":
                                    case "decimal":
                                        if (paramArray['intStart' + i]) searchField.intStart = paramArray['intStart' + i];
                                        if (paramArray['intEnd' + i]) searchField.intEnd = paramArray['intEnd' + i];
                                        break;
                                    case "date":
                                    case "datetime":
                                        if (paramArray['dateStart' + i]) searchField.dateStart = paramArray['dateStart' + i];
                                        if (paramArray['dateEnd' + i]) searchField.dateEnd = paramArray['dateEnd' + i];
                                        break;
                                    default:
                                        if (paramArray['text' + i]) searchField.text = paramArray['text' + i];
                                        break;
                                }
                                naite.searchVuemodel.$data.searchFields.push(searchField);
                            }
                        }
                        getPage(naite.vuemodel.$data.pageNumber || 1);
                    })
                    .fail(function (error) {
                        console.log(error);
                        // naite.handleErrorで、naite.loading(false);
                        // をしている
                        var msg = naite.handleError(error);
                        if (msg) {
                            swal(msg, "", "error");
                        }
                    });
            },
            addField: function () {
                console.log('addField click');
                naite.addFieldVuemodel.$data.name = null;
                naite.addFieldVuemodel.$data.type = null;
                naite.addFieldVuemodel.$data.position = 2;
                naite.addFieldVuemodel.$validator.reset();
                $('#addFieldModal').modal('show');
            },
            addRow: function () {
                naite.post(naite.apiUrls.itemRows, null)
                    .done(function (data) {
                        console.log(data);
                        naite.vuemodel.$data.itemRows.push(data);
                    })
                    .fail(function (error) {
                        console.log(error);
                        var msg = naite.handleError(error);
                        if (msg) {
                            swal(msg, "", "error");
                        }
                    });
            },
            download: function () {
                // HTMLテーブルを取得
                var table = document.getElementById('itemTable');

                // テーブルを配列データに変換する関数
                function tableToArray(table) {
                    var data = [];
                    for (var i = 0; i < table.rows.length; i++) {
                        var row = [];
                        for (var j = 0; j < table.rows[i].cells.length; j++) {
                            var cell = table.rows[i].cells[j];
                            // セル内に<input>がある場合はその値を取得、それ以外はセルのテキストを取得
                            var input = cell.querySelector('input');
                            var links = cell.querySelectorAll('a.file');
                            var filenames = [];
                            links.forEach(function (link) {
                                filenames.push(link.innerText.trim());
                            });
                            var fileName = filenames.join(',');
                            row.push(input ? input.value : fileName ? fileName : cell.innerText.replace(''));
                        }
                        data.push(row);
                    }
                    return data;
                }

                // テーブルから配列データを取得
                var data = tableToArray(table);

                // 配列データをワークシートに変換
                var worksheet = XLSX.utils.aoa_to_sheet(data);

                // ワークブックを作成
                var workbook = XLSX.utils.book_new();
                XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

                // ワークブックをExcelファイルとしてダウンロード
                XLSX.writeFile(workbook, 'items.xlsx');
            },
            next: function (index, event) {
                event.stopPropagation();
                var myField = this.fields[index];
                var replaceField = this.fields[index + 1];
                var param = {
                    ReplaceItemFieldId: replaceField.id
                };
                naite.put(naite.apiUrls.sortItemField(myField.id), param)
                    .done(function (data) {
                        console.log(data);
                        naite.redirect(naite.urls.home);
                    })
                    .fail(function (error) {
                        console.log(error);
                        var msg = naite.handleError(error);
                        if (msg) {
                            swal(msg, "", "error");
                        }
                    });
            },
            prev: function (index, event) {
                event.stopPropagation();
                var myField = this.fields[index];
                var replaceField = this.fields[index - 1];
                var param = {
                    ReplaceItemFieldId: replaceField.id
                };
                naite.put(naite.apiUrls.sortItemField(myField.id), param)
                    .done(function (data) {
                        console.log(data);
                        naite.redirect(naite.urls.home);
                    })
                    .fail(function (error) {
                        console.log(error);
                        var msg = naite.handleError(error);
                        if (msg) {
                            swal(msg, "", "error");
                        }
                    });
            },

            sortBy(key, order, event) {
                event.stopPropagation();
                this.sortKey = key;
                this.sortOrder = order;
                getPage(1);
            },
            changeItem(rowIndex, key) {
                const currentItem = this.itemRows[rowIndex].items[key];
                this.changedData.push(currentItem);
            },
            dynamicField(type) {
                // 条件に応じてフィールドを切り替え
                var field = "valueText";
                switch (type) {
                    case "int":
                        field = "valueInt";
                        break;
                    case "decimal":
                        field = "valueDecimal"
                        break;
                    case "date":
                        field = "valueDateTime";
                        break;
                    case "datetime":
                        field = "valueDateTime"
                        break;
                    default:
                        break;
                }
                return field;
            },
            clickCol: function (field) {
                naite.updateFieldVuemodel.$data.id = field.id;
                naite.updateFieldVuemodel.$data.name = field.name;
                naite.updateFieldVuemodel.$data.fixedFieldType = field.fixedFieldType;
                naite.updateFieldVuemodel.$data.isSearch = field.isSearch;
                naite.updateFieldVuemodel.$data.excelColumnName = field.excelColumnName;
                naite.updateFieldVuemodel.$validator.reset();
                $('#updateFieldModal').modal('show');
            },
            deleteRow: function (row, index) {
                swal({
                    title: "行No." + (index + 1) + "を削除しますか？",
                    text: "",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonText: "はい",
                    cancelButtonText: "いいえ"
                }).then(function (isConfirm) {
                    if (isConfirm.value) {
                        naite.delete(naite.apiUrls.itemRow(row.id), null)
                            .done(function (data) {
                                console.log(data);
                                swal({
                                    title: "削除が完了しました",
                                    text: "",
                                    type: "success"
                                }).then(function () {
                                    naite.redirect(naite.urls.home);
                                });
                            })
                            .fail(function (error) {
                                console.log(error);
                                swal("削除に失敗しました", "", "error");
                            });
                    } else {
                        swal("キャンセルしました", "", "error");
                    }
                });
            },
            uploadFile: function (item, rowIndex, colIndex) {
                naite.uploadFileVuemodel.$data.itemId = item.id;
                naite.uploadFileVuemodel.$data.rowIndex = rowIndex;
                naite.uploadFileVuemodel.$data.colIndex = colIndex;
                $('#uploadFileModal').modal('show');
            },
            selectFile: function (rowIndex, colIndex) {
                naite.selectvuemodel.$data.rowIndex = rowIndex;
                naite.selectvuemodel.$data.colIndex = colIndex;
                naite.selectvuemodel.$data.selecteds = [ ...this.itemRows[rowIndex].items[colIndex].files ];
                $('#selectModal').modal('show');
            },
            deleteFile: function (rowIndex, colIndex, fileIndex) {
                this.itemRows[rowIndex].items[colIndex].files.splice(fileIndex, 1);
                this.changedData.push(this.itemRows[rowIndex].items[colIndex]);
            },
            search: function () {
                $('#searchModal').modal('show');
            },
            importExcel: function () {
                naite.importVuemodel.$data.uploadFile = null;
                naite.importVuemodel.$data.importFile = null;
                naite.importVuemodel.$data.importFileName = null;
                naite.importVuemodel.$validator.reset();
                $('#importModal').modal('show');
            },
            updateFilter: function () {
                getPage(1);
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {
                            var uniqueData = this.changedData.filter((item, index, self) =>
                                index === self.findIndex(t => t.id === item.id)
                            );
                            for (i = 0; i < uniqueData.length; i++) {
                                if (uniqueData[i].valueDateTime === '') {
                                    this.$set(uniqueData[i], 'valueDateTime', null);
                                }
                                if (uniqueData[i].valueDecimal === '') {
                                    this.$set(uniqueData[i], 'valueDecimal', null);
                                }
                                if (uniqueData[i].valueInt === '') {
                                    this.$set(uniqueData[i], 'valueInt', null);
                                }
                            }
                            console.log('Changed Data:', this.changedData);
                            console.log('unique Data:', uniqueData);
                            swal({
                                title: "変更を保存しますか？",
                                text: "",
                                type: "warning",
                                showCancelButton: true,
                                confirmButtonClass: "btn btn-teal",
                                confirmButtonText: "はい",
                                cancelButtonClass: "btn btn-default",
                                cancelButtonText: "キャンセル"
                            }).then(function (isConfirm) {
                                if (isConfirm.value) {
                                    // 二重クリック防止
                                    naite.loading(true);

                                    naite.post(naite.apiUrls.items, uniqueData)
                                        .done(function (data) {
                                            console.log(data);
                                            // 二重クリック解除
                                            naite.loading(false);
                                            swal({
                                                title: "変更を保存しました",
                                                text: "",
                                                type: "success"
                                            }).then(function () {
                                                naite.redirect(naite.urls.home);
                                            });
                                        })
                                        .fail(function (error) {
                                            console.log(error);
                                            var msg = naite.handleError(error);
                                            if (msg) {
                                                swal(msg, "", "error");
                                            }
                                        });

                                } else {
                                    swal.closeModal();
                                }
                            });
                        }
                    });
            }
        }
    });

    // 検索条件
    naite.searchVuemodel = new Vue({
        el: '#searchForm',
        data: {
            searchFields: []
        },
        mounted: function () {
            naite.searchVuemodel = this;
        },
        methods: {
            cancel: function () {
                $('#searchModal').modal('hide');
            },
            clear: function () {
                for (i = 0; i < this.searchFields.length; i++) {
                    this.$set(this.searchFields[i], 'text', null);
                    this.$set(this.searchFields[i], 'intStart', null);
                    this.$set(this.searchFields[i], 'intEnd', null);
                    this.$set(this.searchFields[i], 'dateStart', null);
                    this.$set(this.searchFields[i], 'dateEnd', null);
                }
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {
                            console.log(this.searchFields);
                            $('#searchModal').modal('hide');
                            getPage(1);
                        }
                    });
            }
        }
    });

    function getPage(pageNumber) {
        // 二重クリック防止
        //naite.loading(true);

        // パラメータ
        var item = JSON.parse(JSON.stringify(naite.vuemodel.$data));
        item.pageNumber = pageNumber;
        delete item.fields;
        delete item.itemRows;
        delete item.originalData;
        delete item.changedData;

        // URLに新しいクエリストリングを付与
        var query = "";
        if (item.pageSize) query += "&pageSize=" + encodeURIComponent(item.pageSize);
        if (item.pageNumber) query += "&pageNumber=" + encodeURIComponent(item.pageNumber);
        if (item.sortKey) query += "&sortKey=" + encodeURIComponent(item.sortKey);
        if (item.sortOrder) query += "&sortOrder=" + encodeURIComponent(item.sortOrder);
        
        var searchFields = naite.searchVuemodel.$data.searchFields;
        for (i = 0; i < searchFields.length; i++) {
            switch (searchFields[i].type) {
                case "int":
                case "decimal":
                    if (searchFields[i].intStart) {
                        query += "&intStart" + i + "=" + encodeURIComponent(searchFields[i].intStart);
                    } else {
                        naite.searchVuemodel.$set(naite.searchVuemodel.$data.searchFields[i], 'intStart', null);
                    }
                    if (searchFields[i].intEnd) {
                        query += "&intEnd" + i + "=" + encodeURIComponent(searchFields[i].intEnd);
                    } else {
                        naite.searchVuemodel.$set(naite.searchVuemodel.$data.searchFields[i], 'intEnd', null);
                    } 
                    break;
                case "date":
                case "datetime":
                    if (searchFields[i].dateStart) {
                        query += "&dateStart" + i + "=" + encodeURIComponent(searchFields[i].dateStart);
                    } else {
                        naite.searchVuemodel.$set(naite.searchVuemodel.$data.searchFields[i], 'dateStart', null);
                    }
                    if (searchFields[i].dateEnd) {
                        query += "&dateEnd" + i + "=" + encodeURIComponent(searchFields[i].dateEnd);
                    } else {
                        naite.searchVuemodel.$set(naite.searchVuemodel.$data.searchFields[i], 'dateEnd', null);
                    } 
                    break;
                default:
                    if (searchFields[i].text) query += "&text" + i + "=" + encodeURIComponent(searchFields[i].text);
                    break;
            }
        }
        if (query.length > 0) query = "?" + query.substr(1);
        window.history.replaceState(null, null, $(location).attr('pathname') + query);
        item.searchFields = JSON.parse(JSON.stringify(naite.searchVuemodel.$data.searchFields));

        console.log(item);

        naite.post(naite.apiUrls.searchItemRows, item)
            .done(function (data, status, xhr) {
                console.log(data);
                // ページング作成
                var _pagination = xhr.getResponseHeader('x-pagination');
                var pagination = JSON.parse(_pagination);
                naite.createPagination(pagination.TotalPages, pagination.CurrentPage, 10);

                naite.vuemodel.$data.itemRows = data;
                naite.vuemodel.$data.originalData = data;

                naite.loading(false);
            })
            .fail(function (error) {
                console.log(error);
                // naite.handleErrorで、naite.loading(false);
                // をしている
                var msg = naite.handleError(error);
                if (msg) {
                    swal(msg, "", "error");
                }
            });
    }

    // 新規フィールド追加
    naite.addFieldVuemodel = new Vue({
        el: '#addFieldForm',
        data: {
            name: null,
            type: null,
            types: [
                { value: 'text', text: 'テキスト' },
                { value: 'int', text: '数値(整数)' },
                { value: 'decimal', text: '数値' },
                { value: 'date', text: '日付' },
                { value: 'datetime', text: '日時' },
                { value: 'file', text: '画像・ファイル' },
                /*{ value: 'url', text: 'URL' }*/
            ],
            fixedFieldType: null,
            fixedFieldTypes: [
                { value: 'Code', text: '商品コード' },
                { value: 'Name', text: '商品名' },
                { value: 'InventoryDate', text: '棚卸日' },
                { value: 'InventoryQuantity', text: '棚卸数量' },
                { value: 'StockThreshold', text: '在庫閾値' },
            ],
            isSearch: false,
            excelColumnName: null
        },
        mounted: function () {
            naite.addFieldVuemodel = this;
        },
        methods: {
            cancel: function () {
                $('#addFieldModal').modal('hide');
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {

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
                                    // 二重クリック防止
                                    naite.loading(true);

                                    // パラメータ
                                    var item = {
                                        Name: naite.addFieldVuemodel.$data.name,
                                        Type: naite.addFieldVuemodel.$data.type,
                                        FixedFieldType: naite.addFieldVuemodel.$data.fixedFieldType,
                                        IsSearch: naite.addFieldVuemodel.$data.isSearch,
                                        ExcelColumnName: naite.addFieldVuemodel.$data.excelColumnName
                                    }
                                    console.log(item);

                                    naite.post(naite.apiUrls.itemFields, item)
                                        .done(function (data) {
                                            console.log(data);
                                            // 二重クリック解除
                                            naite.loading(false);
                                            swal({
                                                title: "登録が完了しました",
                                                text: "",
                                                type: "success"
                                            }).then(function () {
                                                naite.redirect(naite.urls.home);
                                            });
                                        })
                                        .fail(function (error) {
                                            console.log(error);
                                            var msg = naite.handleError(error);
                                            if (msg) {
                                                swal(msg, "", "error");
                                            }
                                        });

                                } else {
                                    swal.closeModal();
                                }
                            });
                        }
                    });
            }
        }
    });

    // フィールド更新
    naite.updateFieldVuemodel = new Vue({
        el: '#updateFieldForm',
        data: {
            id: null,
            name: null,
            fixedFieldType: null,
            fixedFieldTypes: [
                { value: 'Code', text: '商品コード' },
                { value: 'Name', text: '商品名' },
                { value: 'InventoryDate', text: '棚卸日' },
                { value: 'InventoryQuantity', text: '棚卸数量' },
                { value: 'StockThreshold', text: '在庫閾値' },
            ],
            isSearch: false,
            excelColumnName: null
        },
        mounted: function () {
            naite.updateFieldVuemodel = this;
        },
        methods: {
            cancel: function () {
                $('#updateFieldModal').modal('hide');
            },
            deleteItem: function () {
                swal({
                    title: this.name + "を削除しますか？",
                    text: "",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonText: "はい",
                    cancelButtonText: "いいえ"
                }).then(function (isConfirm) {
                    if (isConfirm.value) {
                        naite.delete(naite.apiUrls.itemField(naite.updateFieldVuemodel.$data.id), null)
                            .done(function (data) {
                                console.log(data);
                                swal({
                                    title: "削除が完了しました",
                                    text: "",
                                    type: "success"
                                }).then(function () {
                                    naite.redirect(naite.urls.home);
                                });
                            })
                            .fail(function (error) {
                                console.log(error);
                                swal("削除に失敗しました", "", "error");
                            });
                    } else {
                        swal("キャンセルしました", "", "error");
                    }
                });
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {

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
                                    // 二重クリック防止
                                    naite.loading(true);

                                    // パラメータ
                                    var item = {
                                        Id: naite.updateFieldVuemodel.$data.id,
                                        Name: naite.updateFieldVuemodel.$data.name,
                                        FixedFieldType: naite.updateFieldVuemodel.$data.fixedFieldType,
                                        IsSearch: naite.updateFieldVuemodel.$data.isSearch,
                                        ExcelColumnName: naite.updateFieldVuemodel.$data.excelColumnName
                                    }
                                    console.log(item);

                                    naite.put(naite.apiUrls.itemField(item.Id), item)
                                        .done(function (data) {
                                            console.log(data);
                                            // 二重クリック解除
                                            naite.loading(false);
                                            swal({
                                                title: "登録が完了しました",
                                                text: "",
                                                type: "success"
                                            }).then(function () {
                                                naite.redirect(naite.urls.home);
                                            });
                                        })
                                        .fail(function (error) {
                                            console.log(error);
                                            var msg = naite.handleError(error);
                                            if (msg) {
                                                swal(msg, "", "error");
                                            }
                                        });

                                } else {
                                    swal.closeModal();
                                }
                            });
                        }
                    });
            }
        }
    });

    // ファイルのアップロード
    $(function () {
        naite.uploadFileVuemodel = new Vue({
            el: '#uploadFileForm',
            data: {
                itemId: null,
                rowIndex: null,
                colIndex: null,
                status: 1,
                uploaded: 0,
                total: 0,
                progress: 0,
                names: [],
                files: []
            },
            mounted: function () {
                naite.uploadFileVuemodel = this;
            },
            watch: {
                uploaded: function (newVal, oldVal) {
                    this.setProgress();
                    if (newVal != 0 && newVal == this.total) {
                        this.status = 4;
                        this.$refs.image.value = null;
                        this.uploaded = 0;
                        this.total = 0;
                        this.progress = 0;
                        var itemFiles = naite.vuemodel.$data.itemRows[this.rowIndex].items[this.colIndex].files;

                        console.log('itemFiles');
                        console.log(itemFiles);
                        for (let i = 0; i < this.files.length; i++) {
                            var hit = itemFiles.filter(a => a.id == this.files[i].id);
                            if (hit.length == 0) {
                                itemFiles.push(this.files[i]);
                            }
                        }
                        console.log('newFiles');
                        console.log(this.files);
                        this.files = [];
                        const currentItem = naite.vuemodel.$data.itemRows[this.rowIndex].items[this.colIndex];
                        naite.vuemodel.$data.changedData.push(currentItem);
                    }
                }
            },
            methods: {
                cancel: function () {
                    $('#uploadFileModal').modal('hide');
                },
                selectImage: function () {
                    this.$refs.image.click();
                },
                inputImage: function () {
                    this.checkFileName(this.$refs.image.files);
                },
                checkFileName: function (images) {
                    this.status = 2;
                    // 二重クリック防止
                    naite.loading(true);

                    this.names = [];
                    for (i = 0; i < images.length; i++) {
                        this.names.push(images[i].name);
                    }
                    console.log(this.names);

                    naite.post(naite.apiUrls.checkFiles, this.names)
                        .done(function (data) {
                            console.log(data);
                            // 二重クリック解除
                            naite.loading(false);
                            if (data.length > 0) {
                                swal({
                                    title: "既に存在するファイルがあります。\r\n上書きしますか？\r\n\r\n" + data.join('\r\n'),
                                    text: "",
                                    type: "warning",
                                    showCancelButton: true,
                                    confirmButtonText: "はい",
                                    cancelButtonText: "いいえ"
                                }).then(function (isConfirm) {
                                    if (isConfirm.value) {
                                        naite.uploadFileVuemodel.uploadImage(images);
                                    } else {
                                        swal("キャンセルしました", "", "error");
                                    }
                                });
                            }
                            else {
                                naite.uploadFileVuemodel.uploadImage(images);
                            }
                        })
                        .fail(function (error) {
                            console.log(error);
                            var msg = naite.handleError(error);
                            if (msg) {
                                swal(msg, "", "error");
                            }
                        });
                },
                uploadImage: function (images) {
                    console.log('uploadImage');
                    this.status = 3;
                    this.uploaded = 0;
                    this.total = images.length;
                    for (let i = 0; i < images.length; i++) {
                        console.log('1');
                        let formData = new FormData();
                        formData.append('file', images[i]);
                        console.log('2');
                        naite.FileUpload(naite.apiUrls.files, formData)
                            .done(function (data) {
                                console.log(data);
                                naite.uploadFileVuemodel.$data.uploaded++;
                                naite.uploadFileVuemodel.$data.files.push(data);
                            })
                            .fail(function (error) {
                                console.log(error);
                                naite.handleError(error);
                                naite.uploadFileVuemodel.$data.status = 4;
                                naite.uploadFileVuemodel.$refs.image.value = null;
                                naite.uploadFileVuemodel.$data.uploaded = 0;
                                naite.uploadFileVuemodel.$data.total = 0;
                                naite.uploadFileVuemodel.$data.progress = 0;
                                alert('アップロードに失敗したファイルがあります。');
                            });
                    }
                },
                dropImage: function (event) {
                    console.log('dropImage');
                    let images = event.dataTransfer.files;
                    this.checkFileName(images);
                },
                setProgress: function () {
                    if (this.uploaded == 0) {
                        this.progress = 0;
                    } else {
                        this.progress = (this.uploaded / this.total) * 100;
                    }
                    console.log(this.progress);
                },
                resetStatus: function () {
                    this.status = 1;
                }
            }
        });
    });

    $('#selectModal').on('show.bs.modal', function (e) {
        naite.initPagination(getSelectPage);
        getSelectPage(1);
        naite.selectvuemodel.$validator.reset();
    });

    $('#selectModal').on('hidden.bs.modal', function (e) {
        naite.initPagination(getPage);
        naite.vuemodel.$data.itemRows[naite.selectvuemodel.$data.rowIndex].items[naite.selectvuemodel.$data.colIndex].files = [...naite.selectvuemodel.$data.selecteds];
        const currentItem = naite.vuemodel.$data.itemRows[naite.selectvuemodel.$data.rowIndex].items[naite.selectvuemodel.$data.colIndex];
        naite.vuemodel.$data.changedData.push(currentItem);
    });

    naite.selectvuemodel = new Vue({
        el: '#selectForm',
        data: {
            pageSize: 100,
            orderBy: 'name asc',
            pageNumber: 1,
            isCheckAll: false,
            rowIndex: null,
            colIndex: null,
            selecteds: [],
        },
        mounted: function () {
            naite.selectvuemodel = this;
        },
        watch: {
            isCheckAll: function (newVal, oldVal) {
                var obj = $(".select-checkbox");
                if (newVal == true) {
                    obj.attr('checked', true).prop('checked', true).change();
                }
                else {
                    obj.removeAttr('checked').prop('checked', false).change();
                }
            }
        },
        methods: {
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {
                        }
                    });
            }
        }
    });

    function getSelectPage(pageNumber) {

        // パラメータ
        var item = JSON.parse(JSON.stringify(naite.selectvuemodel.$data));
        item.pageNumber = pageNumber;
        delete item.selecteds;
        delete item.isCheckAll;

        naite.get(naite.apiUrls.files, item)
            .done(function (data, status, xhr) {
                console.log(data);
                // ページング作成
                var _pagination = xhr.getResponseHeader('x-pagination');
                var pagination = JSON.parse(_pagination);
                naite.createPagination(pagination.TotalPages, pagination.CurrentPage, 10);

                $('.selectTable tbody').empty();
                for (var i = 0; i < data.length; i++) {
                    var isChecked = false;
                    var selectedId = naite.selectvuemodel.$data.selecteds.filter(a => a.id == data[i].id);
                    if (selectedId.length > 0) {
                        isChecked = true;
                    }
                    $('.selectTable tbody').append(
                        $('<tr></tr>')
                            .append(
                                $('<td class="p-0"></td>')
                                    .append(
                                        $('<div class="checkbox-radios"></div>')
                                            .append(
                                                $('<div class="form-check mb-0"></div>')
                                                    .append(
                                                        $('<label class="form-check-label"></label>')
                                                            .append($('<input class="form-check-input select-checkbox" type="checkbox">').attr('value', data[i].id).attr('checked', isChecked).attr('data-name', data[i].name).attr('data-url', data[i].url))
                                                            .append($('<span class="form-check-sign"></span>').append($('<span class="check"></span>')))
                                                    )
                                            )
                                    )
                            )
                            .append(
                                $('<td></td>').text(data[i].name)
                            )
                    );
                }
            })
            .fail(function (error) {
                console.log(error);
                // naite.handleErrorで、naite.loading(false);
                // をしている
                var msg = naite.handleError(error);
                if (msg) {
                    swal(msg, "", "error");
                }
            });
    }

    $(document).on("change", "input.select-checkbox", function (event) {
        var checked = $(this).prop('checked');
        var id = $(this).val();
        var name = $(this).attr('data-name');
        var url = $(this).attr('data-url');
        if (checked == true) {
            var selectedId = naite.selectvuemodel.$data.selecteds.filter(a => a.id == id);
            if (selectedId.length == 0) {
                naite.selectvuemodel.$data.selecteds.push({ id: id, name: name, url: url });
            }
        } else {
            var deleted = naite.selectvuemodel.$data.selecteds.filter(a => a.id != id);
            naite.selectvuemodel.$data.selecteds = deleted;
        }
    });

    // 商品取込
    naite.importVuemodel = new Vue({
        el: '#importForm',
        data: {
            uploadFile: null,
            importFile: null,
            importFileName: null,
            isHeader: false,
        },
        mounted: function () {
            naite.importVuemodel = this;
        },
        methods: {
            cancel: function () {
                $('#importModal').modal('hide');
            },
            selectedFile: async function (e) {
                e.preventDefault();
                let files = e.target.files;
                naite.importVuemodel.$data.importFile = files[0];
                naite.importVuemodel.$data.importFileName = files[0].name;
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {
                            swal({
                                title: "商品を取込みますか？",
                                text: "",
                                type: "warning",
                                showCancelButton: true,
                                confirmButtonClass: "btn btn-teal",
                                confirmButtonText: "はい",
                                cancelButtonClass: "btn btn-default",
                                cancelButtonText: "キャンセル"
                            }).then(function (isConfirm) {
                                if (isConfirm.value) {
                                    // 二重クリック防止
                                    naite.loading(true);

                                    let formData = new FormData();
                                    formData.append('file', naite.importVuemodel.$data.importFile);
                                    var url = naite.apiUrls.importItemRows + '?isHeader=' + naite.importVuemodel.$data.isHeader;
                                    naite.FileUpload(url , formData)
                                        .done(function (data) {
                                            naite.loading(false);
                                            swal({
                                                title: "商品取込が完了しました",
                                                text: "",
                                                type: "success"
                                            }).then(function () {
                                                naite.redirect($(location).attr('href'));
                                            });
                                        })
                                        .fail(function (error) {
                                            console.log(error);
                                            var msg = naite.handleError(error);
                                            if (msg) {
                                                swal(msg, "", "error");
                                            }
                                        });

                                } else {
                                    swal.closeModal();
                                }
                            });
                        }
                    });
            }
        }
    });

    
});