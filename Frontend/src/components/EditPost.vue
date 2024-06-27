<!-- 发帖界面v2.3 -->
<template>
  <div class="common-layout">
    <my-nav></my-nav>

    <el-container>
      <el-aside width="400px"></el-aside>

      <el-main>
        <div class="icon-text-container">
          <el-icon class="edit-icon">
            <Edit />
          </el-icon>
          <span class="text-content">发布新帖</span>
        </div>

        <div class="title-tag-container">
          <el-input v-model="postTitle" placeholder="请输入帖子标题" size="large" class="edit-title" clearable>
          </el-input>
          <div class="tag-container">
            <el-icon>
              <CollectionTag />
            </el-icon>
            <el-dropdown @command="handleCommand">
              <span class="el-dropdown-link">
                标签<el-icon class="el-icon--right"><arrow-down /></el-icon>
              </span>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="diy">自定义标签</el-dropdown-item>
                  <el-dropdown-item command="西甲">西甲</el-dropdown-item>
                  <el-dropdown-item command="德甲">德甲</el-dropdown-item>
                  <el-dropdown-item command="意甲">意甲</el-dropdown-item>
                  <el-dropdown-item command="法甲">法甲</el-dropdown-item>
                  <el-dropdown-item command="中超">中超</el-dropdown-item>
                  <el-dropdown-item command="英超">英超</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </div>

        <!-- Show selected tags -->
        <div class="selected-tags-container">
          <div v-for="(tag, index) in selectedTags" :key="index" class="selected-tag">
            <template v-if="tag === ''">
              <el-input v-model="diyTagInput" placeholder="请输入自定义标签" @keyup.enter="addDIYTag(index)" clearable>
              </el-input>
            </template>
            <template v-else>
              {{ tag }} <span @click="removeTag(tag)">x</span>
            </template>
          </div>
        </div>
        <div class="text-container">
          <el-input v-model="postText" :rows="15" type="textarea" placeholder="请填写内容" />
        </div>

        <div class="user-container">
          <el-button v-if="selectedPics.length < 3" type="primary" @click="uploadPics">
            上传图片
            <input ref="fileInput" type="file" style="display: none" @change="handleFileChange" accept="image/*"
              multiple />
          </el-button>
          <el-button v-else type="primary" @click="showMaxImageAlert">
            上传图片
          </el-button>
          <el-button type="text" icon="Delete" class="delete-button" @click="deleteDraft">删除草稿</el-button>
          <div class="pic-wrapper"></div>
          <div class="selected-pics-container">
            <div v-for="pic in showPics" :key="pic.name">
              <img :src="pic" alt="Selected Pic" width="100" height="100">
            </div>
          </div>
        </div>

        <div class="post-button">
          <el-button type="primary" @click="post_to_forum">点击发帖</el-button>
        </div>
      </el-main>

      <el-aside width="400px"></el-aside>
    </el-container>
  </div>
</template>
  
<script>
import axios from 'axios';
import MyNav from './nav.vue';
import { ElMessage, ElMessageBox } from 'element-plus';

export default {
  components: {
    'my-nav': MyNav
  },
  data() {
    return {
      postPicsUrl: [],
      postTitle: '',
      postText: '',
      //postTime: '',
      selectedTags: [],
      selectedPics: [],
      showPics: [],
      diyTagInput: '', //用于存储自定义标签的输入内容
    };
  },
  methods: {
    uploadPics() {
      this.$refs.fileInput.click();
    },
    handleFileChange(event) {
      const files = event.target.files;
      for (const file of files) {
        if (this.selectedPics.length >= 3) {
          break;
        }
        this.selectedPics.push(file);
        this.showPics.push(URL.createObjectURL(file));
        const reader = new FileReader();
        reader.onload = (e) => {
          this.postPicsUrl.push(e.target.result);
        };
        reader.readAsDataURL(file);
      }
    },
    showMaxImageAlert() {
      this.$message({
        message: '最多只能上传三张图片',
        type: 'warning'
      });
    },
    //以上为选择图片
    addDIYTag(index) {
      const tag = this.diyTagInput.trim(); // 去除输入内容中的首尾空格
      // 检查输入内容不为空，并且标签在selectedTags中不存在
      if (tag !== '' && !this.selectedTags.includes(tag)) {
        this.selectedTags.splice(index, 1, tag); // 将空标签替换为自定义标签
        this.diyTagInput = ''; // 添加标签后清空输入框
      }
    },
    handleCommand(command) {
      if (command === 'diy') {
        // Custom tag handling
        this.selectedTags.push('');
      } else {
        const tag = `${command}`;
        if (!this.selectedTags.includes(tag)) {
          this.selectedTags.push(tag);
        }
      }
    },
    async post_to_forum() {
      // 首先去除标题和内容两端的空格
      const trimmedTitle = this.postTitle.trim();
      const trimmedText = this.postText.trim();

      // 判断标题和内容是否为空
      if (trimmedTitle === '' || trimmedText === '') {
        this.$message({
          message: '标题和内容均不能为空',
          type: 'error',
        });
        return; // 如果标题或内容为空，不继续执行发帖操作
      }

      if (this.selectedPics.length > 0) {
        await this.submitPics();
      }

      const token = localStorage.getItem('token');
      let response;
      try {
        const headers = {
          Authorization: `Bearer ${token}`,
        };
        response = await axios.post('/api/Forum/NewPost', {
          title: String(this.postTitle),
          contains: String(this.postText),
          tags: this.selectedTags,
          pic: this.postPicsUrl,
        }, { headers });
        console.log('response', response);
      } catch (err) {
        if (err.response.data.result == 'fail') {
          ElMessage({
            message: err.response.data.msg,
            grouping: false,
            type: 'error',
          });
        } else {
          ElMessage({
            message: '发帖失败',
            grouping: false,
            type: 'error',
          });
          return;
        }
        return;
      }
      if (response.data.ok == 'yes') {
        ElMessageBox.alert('发帖成功', '提示', {
          confirmButtonText: '确定',
          callback: () => {
            this.$router.push('/forum');
          }
        });
      } else {
        ElMessage({
          message: '发帖失败',
          grouping: false,
          type: 'error',
        });
      }
    },
    async submitPics() {
      let response
      const formData = new FormData();
      for (const file of this.selectedPics) {
        formData.append('files', file);
      };
      try {
        response = await axios.post('/api/Picture/SaveFile', formData, { 'Content-Type': 'multipart/form-data' });
      } catch (err) {
        console.log(err);
        ElMessage({
          message: '上传图片失败',
          grouping: false,
          type: 'error',
        })
        return
      };
      this.postPicsUrl = response.data.value;
      console.log('图片链接为', this.postPicsUrl);
    },
    deleteDraft() {
      this.postText = '';
      this.selectedPics = [];
      this.showPics = [];
    },
    removeTag(tag) {
      const index = this.selectedTags.indexOf(tag);
      if (index !== -1) {
        this.selectedTags.splice(index, 1);
      }
    }
  },
};
</script>
  
<style scoped>
.common-layout {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

/* 主体部分 */

.el-container {
  flex: 1;
  display: flex;
}

.el-main {
  flex: 1;
  background-color: #F5F7FA;
  padding-top: 20px;
}

/* 编写帖子内容部分 */

.icon-text-container {
  display: flex;
  align-items: center;
  font-weight: bold;
}

/* 图标行 */

.title-tag-container {
  display: flex;
  align-items: center;
  margin-bottom: 30px;
}

/* 标题标签行 */

.tag-container {
  margin-top: 20px;
}

.edit-icon {
  font-size: 24px;
}

/* 图标 */

.text-content {
  margin-left: 8px;
}

/* 使“发布新帖”和icon有一定距离 */

.edit-title {
  margin-top: 20px;
  width: 85%;
  margin-right: 20px;
}

/* 标题的输入框 */

.text-container {
  width: 100%;
}

/* 内容的输入框 */

.user-container {
  margin-top: 10px;
}

/* 上传图片和删除草稿的按钮行 */

.delete-button {
  float: right;
}

/* 使删除草稿按钮在最右侧 */

.pic-wrapper {
  margin-bottom: 20px;
}

/* 使按钮和上传的图片间有间隔 */

.selected-pics-container {
  display: flex;
  flex-wrap: wrap;
}

.selected-pics-container>div {
  margin-right: 10px;
  margin-bottom: 10px;
}

.post-button {
  display: flex;
  justify-content: center;
  margin-top: 10px;
}

.selected-tags-container {
  margin-top: 10px;
  display: flex;
  flex-wrap: wrap;
}

.selected-tag {
  background-color: #e0e0e0;
  border-radius: 3px;
  padding: 2px 5px;
  margin-right: 5px;
  margin-bottom: 5px;
  display: flex;
  align-items: center;
}

.selected-tag span {
  cursor: pointer;
  margin-left: 5px;
}
</style>
