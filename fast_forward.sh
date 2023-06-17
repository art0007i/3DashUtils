#!/bin/bash

my_branch=$(git branch --show-current)
main_branch="master"

git checkout $main_branch
git pull
git checkout $my_branch
git merge $main_branch
git push