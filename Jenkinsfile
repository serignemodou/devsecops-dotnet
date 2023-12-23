pipeline {
    agent "agent-jenkins"
    stages {
        stage ("clone-repo-git"){
            steps {
                echo "Connection to Github and Clone Repo"
                script {
                    withCredentials([gitUsernamePassword(credentialsId: 'auth-github', usernameVariable: 'GIT_USERNAME', passwordVariable: 'GIT_PASSWORD')]){
                        sh 'git status'
                    }
                }
            }
    }
}
}