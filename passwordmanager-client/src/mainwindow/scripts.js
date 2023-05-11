const passwordList = document.querySelector('#password-list ul');
const form = document.querySelector('#password-form');

form.addEventListener('submit', (e) => {
  e.preventDefault();

  const website = document.querySelector('#website').value;
  const username = document.querySelector('#username').value;
  const password = document.querySelector('#password').value;

  const listItem = document.createElement('li');
  listItem.textContent = `${website}: ${username} - ${password}`;

  passwordList.appendChild(listItem);

  form.reset();
});
